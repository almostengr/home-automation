using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Clients;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public class HomeAssistantService : BaseService, IHomeAssistantService
    {
        private readonly ILogger<HomeAssistantService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHomeAssistantClient _client;
        public HomeAssistantService(ILogger<HomeAssistantService> logger, AppSettings appSettings,
            IHomeAssistantClient client) : base(logger, appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
            _client = client;
        }

        public async Task<HaApiResponseDto> TurnOffLightAtBedTimeAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<HaApiResponseDto> TurnOffLightAtSunriseAsync()
        {
            // return await _client.CallHaServiceAsync("light.bed_light", HaServiceRoutes.TurnOffSwitch
            throw new System.NotImplementedException();
        }

        public async Task<HaApiResponseDto> TurnOnLightAtSunsetAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}