using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;

namespace Almostengr.InternetMonitor.Api.Services
{
    public interface IHomeAssistantService : IBaseService
    {
        Task<HaApiResponseDto> TurnOnLightAtSunsetAsync();
        Task<HaApiResponseDto> TurnOffLightAtSunriseAsync();
        Task<HaApiResponseDto> TurnOffLightAtBedTimeAsync();
    }
}