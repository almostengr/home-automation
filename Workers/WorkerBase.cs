using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.DataTransfer;
using Almostengr.InternetMonitor.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.InternetMonitor.Workers
{
    public class WorkerBase : BackgroundService
    {
        private readonly ILogger<WorkerBase> _logger;
        private readonly AppSettings _appSettings;
        private IWebDriver driver = null;
        private string RouterUrl = "";
        private bool _releaseConfig = false;
        private HttpClient _httpClientHA;
        private StringContent _stringContent;

        public WorkerBase(ILogger<WorkerBase> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }

        public int ResetFailCounter()
        {
            return 0;
        }

        public void StartBrowser()
        {
            ChromeOptions options = new ChromeOptions();

#if RELEASE
            _logger.LogInformation("Running in Release mode");

            options.AddArgument("--headless");
            _releaseConfig = true;
#endif

            _logger.LogInformation("Starting the browser");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        public int SetDelayBetweenChecks()
        {
            try
            {
                return Int32.Parse(_appSettings.Router.Interval.ToString());
            }
            catch (Exception)
            {
                return 10;
            }
        }

        public int SetFailCount()
        {
            try
            {
                return Int32.Parse(_appSettings.Router.FailCount.ToString());
            }
            catch (Exception)
            {
                return 3;
            }
        }

        
        public void CloseBrowser()
        {
            if (driver != null)
            {
                driver.Quit();
                _logger.LogInformation("Browser has been closed");
            }
        }

        
        public async Task PostDataToHomeAssistant(string route, string sensorData)
        {
            _logger.LogInformation("Sending data to Home Assistant");

            try
            {
                SensorState sensorState = new SensorState(sensorData);
                var jsonState = JsonConvert.SerializeObject(sensorState).ToLower();
                _stringContent = new StringContent(jsonState, Encoding.ASCII, "application/json");

                _httpClientHA.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _appSettings.HomeAssistant.Token);

                HttpResponseMessage response = await _httpClientHA.PostAsync(route, _stringContent);

                if (response.IsSuccessStatusCode)
                {
                    HaApiResponse haApiResponse =
                        JsonConvert.DeserializeObject<HaApiResponse>(response.Content.ReadAsStringAsync().Result);
                    _logger.LogInformation(response.StatusCode.ToString());
                    _logger.LogInformation("Updated: " + haApiResponse.Last_Updated.ToString());
                }
                else
                {
                    _logger.LogError(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            if (_stringContent != null)
                _stringContent.Dispose();
        }
    }
}