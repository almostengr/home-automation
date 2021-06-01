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
    public abstract class WorkerBase : BackgroundService
    {
        private readonly ILogger<WorkerBase> _logger;
        private readonly AppSettings _appSettings;
        private IWebDriver driver = null;
        private readonly HttpClient _httpClientHA;
        internal readonly int MaxFailCount;

        public WorkerBase(ILogger<WorkerBase> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;

            _httpClientHA = new HttpClient();
            _httpClientHA.BaseAddress = new Uri(_appSettings.HomeAssistant.Url);

            MaxFailCount = SetFailCount();
        }

        public override void Dispose()
        {
            _httpClientHA.Dispose();
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task TaskDelayShort(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }

        public async Task TaskDelayLong(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }

        public IWebDriver StartBrowser()
        {
            ChromeOptions options = new ChromeOptions();

#if RELEASE
            _logger.LogInformation("Running in Release mode");

            options.AddArgument("--headless");
#endif

            _logger.LogInformation("Starting the browser");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

            return driver;
        }

        private int SetFailCount()
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

        public void CloseBrowser(IWebDriver driver)
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
                var jsonState = JsonConvert.SerializeObject(new SensorState(sensorData)).ToLower();
                using (StringContent _stringContent = new StringContent(jsonState, Encoding.ASCII, "application/json"))
                {
                    _httpClientHA.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _appSettings.HomeAssistant.Token);

                    HttpResponseMessage response = await _httpClientHA.PostAsync(route, _stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        HaApiResponse haApiResponse =
                            JsonConvert.DeserializeObject<HaApiResponse>(response.Content.ReadAsStringAsync().Result);
                        _logger.LogInformation("{status}; Updated: {update}", 
                            new string[] {
                                response.StatusCode.ToString(),
                                haApiResponse.Last_Updated.ToString()
                            });
                    }
                    else
                    {
                        _logger.LogError(response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ex.GetType(), ex.Message));
            }
        }
    }
}