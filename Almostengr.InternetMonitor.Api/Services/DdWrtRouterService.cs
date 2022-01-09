using System;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.Services
{
    public class DdWrtRouterService : SeleniumService, IDdWrtRouterService
    {
        private readonly ILogger<DdWrtRouterService> _logger;
        private const int routerRebootSeconds = 120;
        private AppSettings _appSettings;

        public DdWrtRouterService(ILogger<DdWrtRouterService> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task RebootRouterAsync(IWebDriver webDriver = null)
        {
            _logger.LogInformation("Rebooting router");

            try
            {
                if (webDriver == null)
                {
                    webDriver = StartBrowser();
                }

                RouterUrl = SetUrlWithCredentials();

                webDriver.Navigate().GoToUrl(RouterUrl);
                webDriver.FindElement(By.LinkText("Administration")).Click();

                webDriver.FindElement(By.Name("reboot_button")).Click();
#if RELEASE
                webDriver.FindElement(By.Name("reboot_button")).Click();
#endif

                if (webDriver.FindElement(By.TagName("body")).Text.Contains("Unit is rebooting now. Please wait a moment..."))
                {
                    _logger.LogInformation("Router was rebooted");
                    _logger.LogInformation("Waiting {seconds} seconds for reboot to complete", routerRebootSeconds);

                    await Task.Delay(TimeSpan.FromSeconds(routerRebootSeconds));

                    webDriver.Navigate().GoToUrl(RouterUrl);

                    _logger.LogInformation("Router is back online and can be reached");
                }
                else
                {
                    _logger.LogError("Unable to reboot router");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to reboot router");
            }

            CloseBrowser(webDriver);
        } // end function RebootRouterAsync


        public async Task AreWifiDevicesConnectedAsync()
        {
            IWebDriver webDriver = null;

            try
            {
                webDriver = StartBrowser();

                RouterUrl = SetUrlWithCredentials();
                webDriver.Navigate().GoToUrl(RouterUrl);

                string wirelessTableString = webDriver.FindElement(By.Id("wireless_table")).Text;

                _logger.LogInformation("List of connected clients");
                _logger.LogInformation(wirelessTableString);

                int wirelessTableRows = wirelessTableString.ToLower().Split("xx:xx:xx").Length;

                if (wirelessTableRows < _appSettings.Router.MinWirelessClientCount)
                {
                    _logger.LogError(
                        "Less than expected wireless clients are connected. Expected: {expected}, Actual: {actual}",
                        new string[] {
                        _appSettings.Router.MinWirelessClientCount.ToString(),
                        wirelessTableRows.ToString() });

                    await RebootRouterAsync(webDriver);
                }
                else
                {
                    _logger.LogInformation("Wireless clients are connected. {number} devices found", wirelessTableRows);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to check Wifi Devices");
            }

            CloseBrowser(webDriver);
        } // end function AreWifiDevicesConnectedAsync

        public SensorState GetUpTime()
        {
            IWebDriver webDriver = null;
            string uptimeString = string.Empty;

            try
            {
                webDriver = StartBrowser();

                RouterUrl = SetUrlWithCredentials();
                webDriver.Navigate().GoToUrl(RouterUrl);

                uptimeString = webDriver.FindElement(By.Id("uptime")).Text;

                uptimeString = uptimeString.Substring(0, uptimeString.IndexOf(","));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get router uptime");
            }

            CloseBrowser(webDriver);

            return new SensorState(uptimeString);
        }
    }
}