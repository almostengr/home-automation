using System;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Model;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Workers
{
    public class ArrisModemWorker : WorkerBase
    {
        private readonly ILogger<ArrisModemWorker> _logger;
        private readonly AppSettings _appSettings;
        private IWebDriver driver;

        public ArrisModemWorker(ILogger<ArrisModemWorker> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            driver = StartBrowser();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CloseBrowser(driver);
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool modemUp = false;
                _logger.LogInformation("Performing checks at {time}", DateTimeOffset.Now);

                try
                {
                    modemUp = IsModemOperational();

                    await PostDataToHomeAssistant("api/states/sensor.router_modemoperational", modemUp.ToString());

                    await TaskDelayLong(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, string.Concat(ex.GetType(), ex.Message));

                    await PostDataToHomeAssistant("api/states/sensor.router_modemoperational", false.ToString());
                    
                    await TaskDelayShort(stoppingToken);
                }
            }

            CloseBrowser(driver);
        }

        private bool IsModemOperational()
        {
            int errorCount = 0;

            driver.Navigate().GoToUrl(_appSettings.Modem.Url);

            _logger.LogInformation("Checking the modem status page");

            driver.FindElement(By.LinkText("Status")).Click();

            if (driver.PageSource.Contains("OPERATIONAL") == false)
            {
                _logger.LogError("Modem is not in operational state");
                errorCount++;
            }

            _logger.LogInformation("Checking the CM State page");
            
            driver.FindElement(By.LinkText("CM State")).Click();

            IWebElement mainBody = driver.FindElement(By.ClassName("main_body"));
            string mainBodyParagraph = mainBody.FindElement(By.TagName("p")).Text;

            if (mainBodyParagraph.Contains("Docsis-Data Reg Complete") == false)
            {
                _logger.LogError("The modem is not in the expected CM state");
                errorCount++;
            }

            return (errorCount == 0) ? true : false;
        }

    }
}