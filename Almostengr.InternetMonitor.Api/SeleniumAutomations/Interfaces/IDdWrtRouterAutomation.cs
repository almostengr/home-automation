using System.Threading.Tasks;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{
    public interface IDdWrtRouterAutomation : IBaseAutomation
    {
        Task RebootRouterAsync();
        bool AreWifiDevicesConnected();
    }
}