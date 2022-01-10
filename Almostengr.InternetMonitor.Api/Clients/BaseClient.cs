using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Almostengr.InternetMonitor.Api.Clients
{
    public abstract class BaseClient : IBaseClient
    {
        private readonly ILogger<BaseClient> _logger;

        public BaseClient(ILogger<BaseClient> logger)
        {
            _logger = logger;
        }

        public async Task<T> HttpGetAsync<T>(HttpClient httpClient, string route) where T : BaseDto
        {
            HttpResponseMessage response = await httpClient.GetAsync(route);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                _logger.LogError(response.ReasonPhrase);
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<T> HttpPostAsync<T>(HttpClient httpClient, string route, string body) where T : BaseDto
        {
            var json = JsonConvert.SerializeObject(body); // or JsonSerializer.Serialize if using System.Text.Jsonz
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+

            HttpResponseMessage response = await httpClient.PostAsync(route, stringContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                _logger.LogError(response.ReasonPhrase);
                throw new Exception(response.ReasonPhrase);
            }
        }


    }
}