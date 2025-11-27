using System.Threading.Tasks;
using Almostengr.HomeAutomation.Features.Common;
using Almostengr.HomeAutomation.Features.DataTransfer;

namespace Almostengr.HomeAutomation.Features.Services
{
    public interface IHomeAssistantService : IBaseService
    {
        Task<HaApiResponseDto> TurnOnLightAtSunsetAsync();
        Task<HaApiResponseDto> TurnOffLightAtSunriseAsync();
        Task<HaApiResponseDto> TurnOffLightAtBedTimeAsync();
    }
}