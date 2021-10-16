using System;
using Almostengr.InternetMonitor.Api.Models;
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


    }
}