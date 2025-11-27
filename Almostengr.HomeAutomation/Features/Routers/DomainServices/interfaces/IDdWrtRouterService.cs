using Almostengr.HomeAutomation.Features.DataTransfer;
using Almostengr.HomeAutomation.Features.Transcripts.DomainServices.Interfaces;
using OpenQA.Selenium;

namespace Almostengr.HomeAutomation.Features.Routers.DomainServices.interfaces
{
    public interface IDdWrtRouterService : ISeleniumService
    {
        Task RebootRouterAsync(IWebDriver webDriver);
        Task AreWifiDevicesConnectedAsync();
        SensorStateDto GetUpTime();
    }
}