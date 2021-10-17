using System;
using Almostengr.InternetMonitor.Api.Models;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{

    public class HdHomeRunAutomation : BaseAutomation, IHdHomeRunAutomation
    {
        public readonly ILogger<HdHomeRunAutomation> _logger;
        private const string _hdHomeRunUrl = "http://hdhomerun/";

        public HdHomeRunAutomation(ILogger<HdHomeRunAutomation> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
        }

        public bool IsUpdatePending(bool performUpdate = false)
        {
            IWebDriver webDriver = null;
            bool taskSuccessful = false;

            try
            {
                webDriver = StartBrowser();

                webDriver.Navigate().GoToUrl(_hdHomeRunUrl);

                var updateButton = webDriver.FindElement(By.Name("upgrade"));

                if (performUpdate)
                {
                    updateButton.Click();
                }

                taskSuccessful = true;
            }
            catch (NoSuchElementException)
            {
                _logger.LogInformation("No update pending");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to perform update");
            }

            CloseBrowser(webDriver);

            return taskSuccessful;
        }

        public string SystemStatus()
        {
            IWebDriver webDriver = null;
            string systemStatusString = string.Empty;

            try
            {
                webDriver = StartBrowser();

                webDriver.Navigate().GoToUrl(_hdHomeRunUrl);

                webDriver.FindElement(By.LinkText("System Status")).Click();

                var systemTable = webDriver.FindElement(By.TagName("table"));

                var systemRows = systemTable.FindElements(By.TagName("tr"));

                foreach (var systemRow in systemRows)
                {
                    var systemColumns = systemRow.FindElements(By.TagName("td"));

                    if (systemColumns.Count > 0)
                    {
                        systemStatusString += systemColumns[0].Text + ": " + systemColumns[1].Text + ";";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to perform update");
            }

            CloseBrowser(webDriver);
            return systemStatusString;
        }

        public bool PerformUpdate(){
            return IsUpdatePending(true);
        }

        public string TunerStatus()
        {
            IWebDriver webDriver = null;
            string tunerStatusString = string.Empty;

            try{
                webDriver = StartBrowser();

                webDriver.Navigate().GoToUrl(_hdHomeRunUrl);

                webDriver.FindElement(By.LinkText("Tuner Status")).Click();

                var tunerTable = webDriver.FindElement(By.TagName("table"));

                var tunerRows = tunerTable.FindElements(By.TagName("tr"));

                foreach (var tunerRow in tunerRows)
                {
                    var tunerColumns = tunerRow.FindElements(By.TagName("td"));

                    if (tunerColumns.Count > 0)
                    {
                        tunerStatusString += tunerColumns[0].Text + ": " + tunerColumns[1].Text + ";";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to check tuner status");
            }

            CloseBrowser(webDriver);

            return tunerStatusString;
        }
    }
}