using System.Threading.Tasks;
using Almostengr.HomeAutomation.Features.Transcripts.DomainServices.Interfaces;

namespace Almostengr.HomeAutomation.Features.HdHomeRuns.DomainServices
{
    public interface IHdHomeRunService : ISeleniumService
    {
        string SystemStatus();
        Task<bool> IsUpdatePendingAsync(bool performUpdate = false);
        Task<bool> PerformUpdateAsync();
        string TunerStatus();
    }
}