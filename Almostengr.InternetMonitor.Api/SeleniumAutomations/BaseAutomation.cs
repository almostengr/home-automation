using System;
using Almostengr.InternetMonitor.Api.Models;
using Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{
    public abstract class BaseAutomation : IBaseAutomation
    {
        private readonly ILogger<BaseAutomation> _logger;
        private readonly AppSettings _appSettings;
        internal string RouterUrl = "http://router/";

        public BaseAutomation(ILogger<BaseAutomation> logger, AppSettings appSettings){
            _logger = logger;
            _appSettings = appSettings;
        }

        public IWebDriver StartBrowser()
        {
            ChromeOptions options = new ChromeOptions();

#if RELEASE
            _logger.LogInformation("Running in Release mode");

            options.AddArgument("--headless");
#endif

            _logger.LogInformation("Starting the browser");

            IWebDriver driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            return driver;
        }

        public void CloseBrowser(IWebDriver driver)
        {
            if (driver != null)
            {
                driver.Quit();
                _logger.LogInformation("Browser has been closed");
            }
        }

        public string SetRouterUrl()
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

        
    }
}