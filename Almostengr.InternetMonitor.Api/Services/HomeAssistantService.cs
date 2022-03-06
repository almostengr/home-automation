using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Clients;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public class HomeAssistantService : BaseService, IHomeAssistantService
    {
        private readonly ILogger<HomeAssistantService> _logger;
        private readonly IHomeAssistantClient _haClient;
        internal string RouterUrl = "http://router/";

        public HomeAssistantService(ILogger<HomeAssistantService> logger,
            IHomeAssistantClient haClient) : base(logger)
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