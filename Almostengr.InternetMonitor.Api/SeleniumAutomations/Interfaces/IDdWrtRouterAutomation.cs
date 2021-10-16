using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{
    public interface IDdWrtRouterAutomation : IBaseAutomation
    {
        Task RebootRouterAsync(IWebDriver webDriver);
        Task AreWifiDevicesConnectedAsync();
    }
}