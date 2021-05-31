using System;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Model;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Workers
{
    public class DdWrtRouterWorker : WorkerBase
    {
        private readonly ILogger<DdWrtRouterWorker> _logger;
        private readonly AppSettings _appSettings;
        private IWebDriver driver = null;
        private readonly string RouterUrl = "";

        public DdWrtRouterWorker(ILogger<DdWrtRouterWorker> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;

            RouterUrl = SetRouterUrl();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            driver = StartBrowser();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int failCounter = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Performing checks at {time}", DateTimeOffset.Now);

                try
                {
                    bool wifiUp = AreWifiDevicesConnected();
                    await PostDataToHomeAssistant("api/states/sensor.router_wifionline", wifiUp.ToString());

                    if (wifiUp == false)
                    {
                        failCounter++;
                        _logger.LogWarning("Fail count: {failCounter}", failCounter);

                        if (failCounter >= MaxFailCount)
                        {
                            await RebootRouter(stoppingToken);
                            failCounter = 0;
                        }
                    }
                    else
                    {
                        failCounter = 0;
                    }

                    await TaskDelayLong(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, string.Concat(ex.GetType(), ex.Message));
                    await TaskDelayShort(stoppingToken);
                }
            }

            CloseBrowser(driver);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CloseBrowser(driver);
            return base.StopAsync(cancellationToken);
        }

        private string SetRouterUrl()
        {
            _logger.LogInformation("Converting router URL");

            if (string.IsNullOrEmpty(_appSettings.Router.Username) == false &&
                string.IsNullOrEmpty(_appSettings.Router.Password) == false)
            {
                string protocol = _appSettings.Router.Url.Substring(0, _appSettings.Router.Url.IndexOf("://"));
                string cleanedUrl = _appSettings.Router.Url.Replace("https://", "").Replace("http://", "");

                return protocol + "://" +
                    _appSettings.Router.Username + ":" +
                    _appSettings.Router.Password + "@" +
                    cleanedUrl;
            }

            return _appSettings.Router.Url;
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
            int routerRebootSeconds = 90;

            _logger.LogInformation("Rebooting router");

            driver.Navigate().GoToUrl(RouterUrl);
            driver.FindElement(By.LinkText("Administration")).Click();

#if RELEASE
                driver.FindElement(By.Name("reboot_button")).Click();
#endif

            if (driver.FindElement(By.TagName("body")).Text.Contains("Unit is rebooting now. Please wait a moment..."))
            {
                _logger.LogInformation("Router was rebooted");
                _logger.LogInformation("Waiting {seconds} seconds for reboot to complete", routerRebootSeconds);

                await Task.Delay(TimeSpan.FromSeconds(routerRebootSeconds), stoppingToken);

                driver.Navigate().GoToUrl(RouterUrl);

                _logger.LogInformation("Router is back online and can be reached");
            }
            else
            {
                _logger.LogError("Unable to reboot router");
            }
        }

    }
}