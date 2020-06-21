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

        static void DebugMessage(string message)
        {
#if DEBUG
            Console.WriteLine("{0} | DEBUG: {1}", DateTime.Now, message);
#endif
        }

        static void LogMessage(string message)
        {
            Console.WriteLine("{0} | {1}", DateTime.Now, message);
        }

        static void Main(string[] args)
        {
            LogMessage("Starting process");

            try
            {
                SetProgramArguments(args);

                SetRouterUrl();

                StartBrowser();

                IsUpTimeValid();

                AreDevicesOffline();

                LogMessage("No issues to report");
            }
            catch (DriverServiceNotFoundException ex)
            {
                LogMessage("Geckodriver could not be found. " + ex.Message);
            }
            catch (UnhandledAlertException ex)
            {
                LogMessage("Credentials were needed but not provided" + ex.Message);
            }
            catch (ApplicationException ex)
            {
                HandleRebootException(ex);
            }
            catch (WebDriverException ex)
            {
                HandleRebootException(ex);
            }
            catch (Exception ex)
            {
                LogMessage("General exception occurred: " + ex.Message);
            }
            finally
            {
                if (_driver != null)
                {
                    LogMessage("Closing browser");
                    _driver.Quit();
                }
            }

            LogMessage("Process completed");
        }

        static bool HandleRebootException(Exception exception)
        {
            LogMessage(exception.GetType() + exception.Message);

            if (MinUptimeElapsed() == false && AreExceptionHostsOnline() == true)
            {
                return false;
            }

            RebootRouter();

            return true;
        }

        static void SetProgramArguments(string[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == "--username")
                {
                    username = arguments[i + 1];
                    DebugMessage("Username provided");
                }

                if (arguments[i] == "--password")
                {
                    password = arguments[i + 1];
                    DebugMessage("Password provided");
                }

                if (arguments[i] == "--hostname")
                {
                    hostname = arguments[i + 1];
                    DebugMessage("Hostname provided");
                }

                if (arguments[i] == "--protocol")
                {
                    protocol = arguments[i + 1];
                    DebugMessage("Protocol provided");
                }
            }
        }

        static void SetRouterUrl()
        {
            if (username != "" && password != "")
            {
                router = protocol + "://" + username + ":" + password + "@" + hostname;
            }
            else
            {
                router = protocol + "://" + hostname;
            }
        }

        static void StartBrowser()
        {
            LogMessage("Starting browser");

            FirefoxOptions firefoxOptions = new FirefoxOptions();
#if RELEASE
            firefoxOptions.AddArguments("--headless");
#endif

            _driver = new FirefoxDriver(firefoxOptions);
            _driver.Navigate().GoToUrl(router);
        }

        private static bool AreDevicesOffline()
        {
            LogMessage("Checking to see if Wifi devices are offline");
            int devicesFound = 0;

            string[] deviceMacToCheck = {
                "E0:76", // energy montior
                "B2:CF"  // thermostat
            };

            string wirelessTableText = _driver.FindElement(By.Id("wireless_table")).Text;

            foreach (var device in deviceMacToCheck)
            {
                if (wirelessTableText.Contains(device))
                {
                    LogMessage("Found device ending with " + device);
                    devicesFound++;
                }
            }

            LogMessage("Devices found online: " + devicesFound + " of " + deviceMacToCheck.Length + " total");

            if (devicesFound == deviceMacToCheck.Length)
            {
                return true;
            }

            throw new ApplicationException("All devices were not found online");
        }

        private static void IsUpTimeValid()
        {
            LogMessage("Checking the router uptime");

            string upTimeText = _driver.FindElement(By.Id("uptime")).Text;
            LogMessage("Uptime: " + upTimeText);

            if (upTimeText.Contains("8 days") || upTimeText.Contains("9 days"))
            {
                throw new ApplicationException("Router uptime was greater than 1 week");
            }
        }

        private static bool MinUptimeElapsed()
        {
            LogMessage("Check to see if the minimum uptime has passed");
            string upTimeText = _driver.FindElement(By.Id("uptime")).Text;

            if (upTimeText.Contains(" min, "))
            {
                LogMessage("Minimum uptime has not elapsed");
                return false;
            }

            return true;
        }

        static bool AreExceptionHostsOnline()
        {
            LogMessage("Checking for exception hosts are online");

            int currentHour = Int32.Parse(DateTime.Now.ToString("HH"));
            if (currentHour >= 0 && currentHour < 7)
            {
                LogMessage("Bypassing check due to night time hours");
                return false;
            }

            string[] exceptionHosts = { "4E:B7", "aeoffice" };

            _driver.Navigate().GoToUrl(router + "/Status_Lan.asp");

            string activeClients = _driver.FindElement(By.Id("active_clients_table")).Text;
            string dhcpLeases = _driver.FindElement(By.Id("dhcp_leases_table")).Text;

            foreach (var exceptionItem in exceptionHosts)
            {
                if (activeClients.Contains(exceptionItem) || dhcpLeases.Contains(exceptionItem))
                {
                    LogMessage("Host was found to be online. " + exceptionItem);
                    return true;
                }
            }

            return false;
        }

        static void RebootRouter()
        {
            LogMessage("Rebooting router");

            LogMessage("Going to Administration page");
            _driver.FindElement(By.LinkText("Administration")).Click();

            LogMessage("Rebooting router");
            _driver.FindElement(By.Name("reboot_button")).Click();

            string bodyText = _driver.FindElement(By.TagName("body")).Text;

            if (bodyText.Contains("Unit is rebooting now. Please wait a moment..."))
            {
                LogMessage("Waiting for router to come back online");
                Thread.Sleep(90 * 100);
            }
            else
            {
                LogMessage("Error occurred when attempting to click reboot button");
            }

            LogMessage("Uptime: " + _driver.FindElement(By.Id("uptime")).Text);
        }
    }
}
