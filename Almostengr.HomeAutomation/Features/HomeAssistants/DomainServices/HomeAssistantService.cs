using Almostengr.HomeAutomation.Features.DataTransfer;
using Almostengr.HomeAutomation.Features.Services;
using Almostengr.InternetMonitor.Api.Services;

namespace Almostengr.HomeAutomation.Features.HomeAssistants
{
    public class HomeAssistantService : BaseService, IHomeAssistantService
    {
        private readonly ILogger<HomeAssistantService> _logger;
        private readonly IHomeAssistantService _haClient;
        internal string RouterUrl = "http://router/";

        public HomeAssistantService(ILogger<HomeAssistantService> logger,
            IHomeAssistantService haClient) : base(logger)
        {
            _logger = logger;
            _haClient = haClient;
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