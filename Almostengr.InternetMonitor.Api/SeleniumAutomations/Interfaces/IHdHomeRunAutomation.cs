namespace Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces
{
    public interface IHdHomeRunAutomation : IBaseAutomation
    {
        string SystemStatus();
        bool IsUpdatePending(bool performUpdate = false);
        bool PerformUpdate();
        string TunerStatus();
    }
}