using System;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.InternetMonitor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private AppSettings _appSettings;
        private IWebDriver driver = null;
        private string RouterUrl = "";
        private bool _releaseConfig = false;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _appSettings = new AppSettings();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting monitor");
            ConfigurationBinder.Bind(_config, _appSettings);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RouterUrl = SetRouterUrl();
            int delayBetweenChecks = SetDelayBetweenChecks();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (driver == null)
                    {
                        StartBrowser();
                    }

                    _logger.LogInformation("Performing checks at {time}", DateTimeOffset.Now);

                    bool wifiUp = AreWifiDevicesConnected();
                    if (wifiUp == false || _releaseConfig == false)
                    {
                        await RebootRouter(stoppingToken);
                    }

                    bool modemUp = IsModemOperational();
                    if (modemUp == false || _releaseConfig == false)
                    {
                        IsWebsiteReachable();
                    }

                    _logger.LogInformation("Sleeping for {delayBetweenChecks} seconds starting at {Now}", 
                        new [] {delayBetweenChecks.ToString(), DateTimeOffset.Now.ToString()});
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

        private void StartBrowser()
        {
            ChromeOptions options = new ChromeOptions();

#if RELEASE
            _logger.LogInformation("Running in Release mode");

            options.AddArgument("--headless");
            _releaseConfig = true;
#endif

            _logger.LogInformation("Starting the browser");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        private int SetDelayBetweenChecks()
        {
            try
            {
                return Int32.Parse(_appSettings.Application.Router.Interval.ToString());
            }
            catch (Exception)
            {
                return 10;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down monitor");
            CloseBrowser();

            return base.StopAsync(cancellationToken);
        }

        private void CloseBrowser()
        {
            if (driver != null)
            {
                driver.Quit();
                _logger.LogInformation("Browser has been closed");
            }
        }

        private bool AreWifiDevicesConnected()
        {
            driver.Navigate().GoToUrl(RouterUrl);

            string wirelessTableString = driver.FindElement(By.Id("wireless_table")).Text;
            int wirelessTableRows = wirelessTableString.ToLower().Split("xx:xx:xx:xx").Length;

            if (wirelessTableRows < _appSettings.Application.Router.MinWirelessClientCount)
            {
                _logger.LogError(string.Concat(
                    "Less than expected wireless clients are connected. Expected: ",
                    _appSettings.Application.Router.MinWirelessClientCount,
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

            _logger.LogInformation("Router was rebooted");
            _logger.LogInformation("Waiting {seconds} seconds for reboot to complete",
                _appSettings.Application.Router.RebootWait);
                
            await Task.Delay(TimeSpan.FromSeconds(_appSettings.Application.Router.RebootWait), stoppingToken);

            driver.Navigate().GoToUrl(RouterUrl);

            _logger.LogInformation("Router is back online and can be reached");
        }

        private string SetRouterUrl()
        {
            _logger.LogInformation("Converting router URL");

            if (string.IsNullOrEmpty(_appSettings.Application.Router.Username) == false &&
                string.IsNullOrEmpty(_appSettings.Application.Router.Password) == false)
            {
                string protocol = _appSettings.Application.Router.Url
                    .Substring(0, _appSettings.Application.Router.Url.IndexOf("://"));
                string cleanedUrl = _appSettings.Application.Router.Url
                    .Replace("https://", "").Replace("http://", "");

                return protocol + "://" +
                    _appSettings.Application.Router.Username + ":" +
                    _appSettings.Application.Router.Password + "@" +
                    cleanedUrl;
            }
            else
            {
                return _appSettings.Application.Router.Url;
            }
        }

        private bool IsModemOperational()
        {
            int count = 0;

            driver.Navigate().GoToUrl(_appSettings.Application.Modem.Url);

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

        private bool IsGoogleReachable()
        {
            _logger.LogInformation("Checking Google");

            driver.Navigate().GoToUrl("https://www.google.com");
            driver.FindElement(By.Name("q")).SendKeys("current date");
            driver.FindElement(By.Name("q")).Submit();

            bool dateStringFound = driver.FindElement(By.TagName("body")).Text.Contains(DateTime.Now.ToLongDateString());
            _logger.LogDebug("Current date: {date}", DateTime.Now.ToLongDateString());

            if (dateStringFound)
            {
                _logger.LogInformation("Successfully checked Google");
                return true;
            }
            else
            {
                _logger.LogError("Failed to check Google");
                return false;
            }
        }

        private bool IsRhtServicesReachable()
        {
            driver.Navigate().GoToUrl("https://rhtservices.net");

            IWebElement contactLinkElement = driver.FindElement(By.LinkText("Contact"));
            if (contactLinkElement.Displayed)
            {
                _logger.LogInformation("Successfully checked RHT Services");
                return true;
            }
            else
            {
                _logger.LogError("Failed to check RHT Services");
                return false;
            }
        }

        private bool IsAmazonReachable()
        {
            _logger.LogInformation("Checking Amazon");
            driver.Navigate().GoToUrl("https://www.amazon.com");

            IWebElement searchBoxElement = driver.FindElement(By.Id("twotabsearchtextbox"));
            if (searchBoxElement.Displayed)
            {
                _logger.LogInformation("Successfully checked Amazon");
                return true;
            }
            else
            {
                _logger.LogError("Failed to check Amazon");
                return false;
            }
        }

        private void IsWebsiteReachable()
        {
            Random random = new Random();
            switch (random.Next(0, 3))
            {
                case 0:
                    IsRhtServicesReachable();
                    break;
                case 1:
                    IsAmazonReachable();
                    break;
                default:
                    IsGoogleReachable();
                    break;
            }
        }
    }
}