using System.Threading.Tasks;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces
{
    public interface IHdHomeRunAutomation : IBaseAutomation
    {
        string SystemStatus();
        Task<bool> IsUpdatePendingAsync(bool performUpdate = false);
        Task<bool> PerformUpdateAsync();
        string TunerStatus();
    }
}