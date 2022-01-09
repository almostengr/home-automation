using System.Threading.Tasks;

namespace Almostengr.InternetMonitor.Api.Services
{
    public interface IHdHomeRunService : ISeleniumService
    {
        string SystemStatus();
        Task<bool> IsUpdatePendingAsync(bool performUpdate = false);
        Task<bool> PerformUpdateAsync();
        string TunerStatus();
    }
}