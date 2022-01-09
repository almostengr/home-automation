using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.InternetMonitor.Api.Services
{
    public abstract class SeleniumService : BaseService, ISeleniumService
    {
        private readonly ILogger<SeleniumService> _logger;

        public SeleniumService(ILogger<SeleniumService> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
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