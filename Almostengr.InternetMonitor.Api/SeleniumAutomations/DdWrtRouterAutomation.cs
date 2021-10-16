using System;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Models;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{
    public class DdWrtRouterAutomation : BaseAutomation, IDdWrtRouterAutomation
    {
        private readonly ILogger<DdWrtRouterAutomation> _logger;
        private const int routerRebootSeconds = 120;
        private AppSettings _appSettings;

        public DdWrtRouterAutomation(ILogger<DdWrtRouterAutomation> logger, AppSettings appSettings) : base(logger, appSettings)
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

                RouterUrl = SetRouterUrl();

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

        private string SetRouterUrl()
        {
            _logger.LogInformation("Converting router URL");

            if (string.IsNullOrEmpty(_appSettings.Router.Username) == false &&
                string.IsNullOrEmpty(_appSettings.Router.Password) == false)
            {
                string protocol = _appSettings.Router.Host.Substring(0, _appSettings.Router.Host.IndexOf("://"));
                string cleanedUrl = _appSettings.Router.Host.Replace("https://", "").Replace("http://", "");

                return protocol + "://" +
                    _appSettings.Router.Username + ":" +
                    _appSettings.Router.Password + "@" +
                    cleanedUrl;
            }

            return _appSettings.Router.Host;
        }

        public async Task AreWifiDevicesConnectedAsync()
        {
            IWebDriver webDriver = null;

            try
            {
                webDriver = StartBrowser();

                RouterUrl = SetRouterUrl();
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

    }
}