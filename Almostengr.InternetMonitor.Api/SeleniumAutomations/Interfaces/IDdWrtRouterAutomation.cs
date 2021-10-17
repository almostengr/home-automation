using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces
{
    public interface IDdWrtRouterAutomation : IBaseAutomation
    {
        Task RebootRouterAsync(IWebDriver webDriver);
        Task AreWifiDevicesConnectedAsync();
        SensorState GetUpTime();
    }
}