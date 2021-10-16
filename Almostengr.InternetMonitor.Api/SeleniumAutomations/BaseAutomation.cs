using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Almostengr.InternetMonitor.Api.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{
    public abstract class BaseAutomation : IBaseAutomation
    {
        private readonly ILogger<BaseAutomation> _logger;
        private readonly AppSettings _appSettings;
        internal string RouterUrl = "http://router/";
        private readonly HttpClient _httpClientHA;

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