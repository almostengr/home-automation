using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;

namespace Almostengr.InternetMonitor.Api.Clients
{
    public interface IHomeAssistantClient : IBaseClient
    {
        Task<HaApiResponseDto> CallHaServiceAsync(string entityName, string serviceRoute, string body);
    }
}