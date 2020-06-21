using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Almostengr.RebootRouter
{
    class Program
    {
        static IWebDriver _driver = null;
        static string router, protocol = "http", username = "", password = "", hostname = "router";

        static void logMessage(string message)
        {
            Console.WriteLine("{0} | {1}", DateTime.Now, message);
        }

        static void Main(string[] args)
        {
            logMessage("Starting process");

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--username")
                {
                    username = args[i + 1];
                    logMessage("Username provided");
                }

                if (args[i] == "--password")
                {
                    password = args[i + 1];
                    logMessage("Password provided");
                }

                if (args[i] == "--hostname")
                {
                    hostname = args[i + 1];
                    logMessage("Hostname provided");
                }

                if (args[i] == "--protocol")
                {
                    protocol = args[i + 1];
                    logMessage("Protocol provided");
                }
            }

            router = (username.Length > 0 && password.Length > 0) ? protocol + "://" + username + ":" + password + "@" + hostname : protocol + "://" + hostname;

            try
            {
                StartBrowser();

                IsSenseOnline();
                IsUpTimeValid();
                IsThermostateOnline();

                logMessage("No issues to report");
            }
            catch (Exception ex)
            {
                logMessage("Exception occurred");
                logMessage(ex.Message);

                if (ex is ApplicationException || ex is WebDriverException)
                {
                    if (AreExceptionHostsOnline() == false)
                    {
                        RebootRouter();
                    }
                }
            }

            logMessage("Closing browser");
            _driver.Quit();

            logMessage("Process completed");
        }

        static void StartBrowser()
        {
            logMessage("Starting browser");

            FirefoxOptions firefoxOptions = new FirefoxOptions();

            _driver = new FirefoxDriver(firefoxOptions);
            _driver.Navigate().GoToUrl(router);
        }

        private static bool IsSenseOnline()
        {
            logMessage("Checking to see if Sense is online");

            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> tableCells = _driver.FindElements(By.TagName("td"));

            foreach (var cell in tableCells)
            {
                if (cell.Text == "sense")
                {
                    logMessage("Sense is connected to router");
                    return true;
                }
            }

            throw new ApplicationException("Sense is NOT connected to router");
        }

        private static bool IsThermostateOnline()
        {
            logMessage("Checking to see if thermostat is online");

            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> tableCells = _driver.FindElements(By.TagName("td"));

            foreach (var cell in tableCells)
            {
                if (cell.Text == "192.168.59.100")
                {
                    logMessage("Thermostat is connected to router");
                    return true;
                }
            }

            throw new ApplicationException("Thermostat is NOT connected to router");
        }

        private static void IsUpTimeValid()
        {
            logMessage("Checking the router uptime");

            string upTimeText = _driver.FindElement(By.Id("uptime")).Text;
            logMessage("Uptime: " + upTimeText);

            if (upTimeText.Contains("8 days") || upTimeText.Contains("9 days"))
            {
                throw new ApplicationException("Router uptime was greater than 1 week");
            }
        }

        static bool AreExceptionHostsOnline()
        {
            logMessage("Checking for exception hosts are online");

            int currentHour = Int32.Parse(DateTime.Now.ToString("HH"));
            if (currentHour >= 0 && currentHour <= 7)
            {
                logMessage("Bypassing check due to night time hours");
                return false;
            }

            string[] exceptHosts = { "xx:xx:xx:xx:4E:B7", "aeoffice" };

            _driver.Navigate().GoToUrl(router + "/Status_Lan.asp");

            var cellElements = _driver.FindElements(By.TagName("td"));

            foreach (var host in exceptHosts)
            {
                foreach (var cell in cellElements)
                {
                    if (host == cell.Text)
                    {
                        logMessage("Host was found to be online");
                        return true;
                    }
                }
            }

            return false;
        }

        static void RebootRouter()
        {
            logMessage("Rebooting router");

            logMessage("Going to Administration page");
            _driver.FindElement(By.LinkText("Administration")).Click();

            logMessage("Rebooting router");
            _driver.FindElement(By.Name("reboot_button")).Click();

            string bodyText = _driver.FindElement(By.TagName("body")).Text;

            if (bodyText.Contains("Unit is rebooting now. Please wait a moment..."))
            {
                logMessage("Waiting for router to come back online");
                Thread.Sleep(60 * 100);
            }
            else
            {
                logMessage("Error occurred when attempting to click reboot button");
            }

            logMessage("Uptime: " + _driver.FindElement(By.Id("uptime")).Text);
        }
    }
}
