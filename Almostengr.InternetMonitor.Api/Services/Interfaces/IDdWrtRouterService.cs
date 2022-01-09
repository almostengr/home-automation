using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.Services
{
    public interface IDdWrtRouterService : ISeleniumService
    {
        Task RebootRouterAsync(IWebDriver webDriver);
        Task AreWifiDevicesConnectedAsync();
        SensorState GetUpTime();
    }
}