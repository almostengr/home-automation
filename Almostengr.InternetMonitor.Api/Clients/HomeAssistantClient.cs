using System;
using System.Net.Http;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Clients
{
    public class HomeAssistantClient : BaseClient, IHomeAssistantClient
    {
        private readonly ILogger<HomeAssistantClient> _logger;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;

        public HomeAssistantClient(ILogger<HomeAssistantClient> logger, AppSettings appSettings) : base(logger)
        {
            _logger = logger;
            _appSettings = appSettings;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_appSettings.HomeAssistant.Url);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_appSettings.HomeAssistant.Token}");
        }

        public async Task<HaApiResponseDto> CallHaServiceAsync(string entityName, string serviceRoute, string body)
        {
            try
            {
                // await _client.HttpPostAsync(_httpClient, serviceRoute, body);
                return await HttpPostAsync<HaApiResponseDto>(_httpClient, serviceRoute, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

    }
}