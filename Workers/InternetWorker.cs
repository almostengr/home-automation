using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.DataTransfer;
using Almostengr.InternetMonitor.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.InternetMonitor.Workers
{
    public class InternetWorker : WorkerBase
    {
        private readonly ILogger<InternetWorker> _logger;
        private readonly AppSettings _appSettings;
        private IWebDriver driver = null;
        private string RouterUrl = "";
        private bool _releaseConfig = false;
        private HttpClient _httpClientHA;
        private StringContent _stringContent;

        public InternetWorker(ILogger<InternetWorker> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;

            _httpClientHA = new HttpClient();
            _httpClientHA.BaseAddress = new Uri(_appSettings.HomeAssistant.Url);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting monitor");
            StartBrowser();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RouterUrl = SetRouterUrl();
            int delayBetweenChecks = SetDelayBetweenChecks();
            int maxFailCount = SetFailCount();
            int failCounter = ResetFailCounter();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Performing checks at {time}", DateTimeOffset.Now);

                    bool wifiUp = AreWifiDevicesConnected();
                    await PostDataToHomeAssistant("api/states/sensor.router_wifionline", wifiUp.ToString());

                    if (wifiUp == false)
                    {
                        failCounter++;
                        _logger.LogWarning("Fail count: {failCounter}", failCounter);

                        if (failCounter >= maxFailCount)
                        {
                            await RebootRouter(stoppingToken);
                            failCounter = ResetFailCounter();
                        }
                    }
                    else
                    {
                        failCounter = ResetFailCounter();
                    }

                    bool modemUp = IsModemOperational();
                    await PostDataToHomeAssistant("api/states/sensor.router_modemoperational", modemUp.ToString());

                    if (modemUp == false)
                    {
                        bool externalAccessible = IsWebsiteReachable();
                        await PostDataToHomeAssistant("api/states/sensor.router_internetconnected", externalAccessible.ToString());
                    }
                    else
                    {
                        await PostDataToHomeAssistant("api/states/sensor.router_internetconnected", true.ToString());
                    }

                    _logger.LogInformation("Sleeping for {delayBetweenChecks} seconds starting at {Now}",
                        new[] { delayBetweenChecks.ToString(), DateTimeOffset.Now.ToString() });
                    await Task.Delay(TimeSpan.FromSeconds(delayBetweenChecks), stoppingToken);
                }
                catch (ElementNotVisibleException ex)
                {
                    _logger.LogError(ex, string.Concat(ex.GetType(), ex.Message));
                }
                catch (WebDriverException ex)
                {
                    _logger.LogError(ex, string.Concat(ex.GetType(), ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, string.Concat(ex.GetType(), ex.Message));
                }

                _logger.LogInformation("Done performing checks at {time}", DateTimeOffset.Now);
            } // end while

            CloseBrowser();
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down monitor");
            CloseBrowser();
            return base.StopAsync(cancellationToken);
        }


        private bool AreWifiDevicesConnected()
        {
            driver.Navigate().GoToUrl(RouterUrl);

            string wirelessTableString = driver.FindElement(By.Id("wireless_table")).Text;
            _logger.LogInformation("List of connected clients");
            _logger.LogInformation(wirelessTableString);
            int wirelessTableRows = wirelessTableString.ToLower().Split("xx:xx:xx:xx").Length;

            if (wirelessTableRows < _appSettings.Router.MinWirelessClientCount)
            {
                _logger.LogError(string.Concat(
                    "Less than expected wireless clients are connected. Expected: ",
                    _appSettings.Router.MinWirelessClientCount,
                    " Actual: ",
                    wirelessTableRows
                ));
                return false;
            }
            else
            {
                _logger.LogInformation("Wireless clients are connected. {number} devices found", wirelessTableRows);
                return true;
            }
        }

        private async Task RebootRouter(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Rebooting router");

            driver.Navigate().GoToUrl(RouterUrl);
            driver.FindElement(By.LinkText("Administration")).Click();

            if (_releaseConfig)
            {
                driver.FindElement(By.Name("reboot_button")).Click();
            }

            if (driver.FindElement(By.TagName("body")).Text.Contains("Unit is rebooting now. Please wait a moment..."))
            {
                _logger.LogInformation("Router was rebooted");
                _logger.LogInformation("Waiting {seconds} seconds for reboot to complete",
                    _appSettings.Router.RebootWait);

                await Task.Delay(TimeSpan.FromSeconds(_appSettings.Router.RebootWait), stoppingToken);

                driver.Navigate().GoToUrl(RouterUrl);

                _logger.LogInformation("Router is back online and can be reached");
            }
            else
            {
                _logger.LogError("Unable to reboot router");
            }
        }

        private string SetRouterUrl()
        {
            _logger.LogInformation("Converting router URL");

            if (string.IsNullOrEmpty(_appSettings.Router.Username) == false &&
                string.IsNullOrEmpty(_appSettings.Router.Password) == false)
            {
                string protocol = _appSettings.Router.Url
                    .Substring(0, _appSettings.Router.Url.IndexOf("://"));
                string cleanedUrl = _appSettings.Router.Url
                    .Replace("https://", "").Replace("http://", "");

                return protocol + "://" +
                    _appSettings.Router.Username + ":" +
                    _appSettings.Router.Password + "@" +
                    cleanedUrl;
            }

            return _appSettings.Router.Url;
        }

        private bool IsModemOperational()
        {
            int count = 0;

            driver.Navigate().GoToUrl(_appSettings.Modem.Url);

            _logger.LogInformation("Checking the modem status page");
            driver.FindElement(By.LinkText("Status")).Click();

            if (driver.PageSource.Contains("OPERATIONAL") == false)
            {
                _logger.LogError("Modem is not in operational state");
                count++;
            }

            _logger.LogInformation("Checking the CM State page");
            driver.FindElement(By.LinkText("CM State")).Click();

            IWebElement mainBody = driver.FindElement(By.ClassName("main_body"));
            string mainBodyParagraph = mainBody.FindElement(By.TagName("p")).Text;

            if (mainBodyParagraph.Contains("Docsis-Data Reg Complete") == false)
            {
                _logger.LogError("The modem is not in the expected CM state");
                count++;
            }

            return (count == 0) ? true : false;
        }

        private bool IsWebsiteReachable()
        {
            bool websiteReached = false;
            Random random = new Random();

            switch (random.Next(0, 5))
            {
                case 0:
                    websiteReached = IsYahooReachable();
                    break;
                case 1:
                    websiteReached = IsAmazonReachable();
                    break;
                case 2:
                    websiteReached = IsFacebookReachable();
                    break;
                case 3:
                    websiteReached = IsTwitterReachable();
                    break;
                default:
                    websiteReached = IsGoogleReachable();
                    break;
            }

            return websiteReached;
        }

        private bool IsGoogleReachable()
        {
            _logger.LogInformation("Checking Google");

            try
            {
                driver.Navigate().GoToUrl("https://www.google.com");
                driver.FindElement(By.Name("q")).SendKeys("current date");
                driver.FindElement(By.Name("q")).Submit();

                bool dateStringFound = driver.FindElement(By.TagName("body")).Text.Contains(DateTime.Now.ToLongDateString());
                _logger.LogDebug("Current date: {date}", DateTime.Now.ToLongDateString());

                _logger.LogInformation("Successfully checked Google");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat("Failed to check Google. ", ex.Message));
                return false;
            }
        }

        private bool IsAmazonReachable()
        {
            _logger.LogInformation("Checking Amazon");

            try
            {
                driver.Navigate().GoToUrl("https://www.amazon.com");

                IWebElement searchBoxElement = driver.FindElement(By.Id("twotabsearchtextbox"));
                if (searchBoxElement.Displayed == false)
                {
                    throw new Exception("Search box was not found on page.");
                }

                _logger.LogInformation("Successfully checked Amazon");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat("Failed to check Amazon. ", ex.Message));
                return false;
            }
        }

        private bool IsYahooReachable()
        {
            _logger.LogInformation("Checking Yahoo Finance");

            try
            {
                driver.Navigate().GoToUrl("https://finance.yahoo.com/");
                string pageBody = driver.FindElement(By.TagName("body")).Text;

                if (pageBody.ToLower().Contains("nasdaq") == false)
                {
                    throw new Exception("Expected text was not found on page.");
                }

                _logger.LogInformation("Successfully checked Yahoo Finance");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat("Failed to check Yahoo Finance. ", ex.Message));
                return false;
            }
        }

        private bool IsTwitterReachable()
        {
            _logger.LogInformation("Checking Twitter");
            try
            {
                driver.Navigate().GoToUrl("https://www.twitter.com");
                string pageBody = driver.FindElement(By.TagName("body")).Text;

                if (pageBody.ToLower().Contains("twitter") == false)
                {
                    throw new Exception("Expected text was not found on page");
                }

                _logger.LogInformation("Successfully checked Twitter");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat("Failed to check Twitter. ", ex.Message));
                return false;
            }
        }

        private bool IsFacebookReachable()
        {
            _logger.LogInformation("Checking Facebook");

            try
            {
                driver.Navigate().GoToUrl("https://www.facebook.com");
                IWebElement emailElement = driver.FindElement(By.Id("email"));
                IWebElement passwordElement = driver.FindElement(By.Id("password"));

                if (emailElement.Displayed == false || passwordElement.Displayed == false)
                {
                    throw new Exception("Expected text was not found on page");
                }

                _logger.LogInformation("Successfully checked Facebook");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat("Failed to check Facebook. ", ex.Message));
                return false;
            }
        }


        public override void Dispose()
        {
            _httpClientHA.Dispose();
            _stringContent.Dispose();
            base.Dispose();
        }

    }
}