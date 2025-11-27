using Almostengr.HomeAutomation.Features.Common;
using OpenQA.Selenium;

namespace Almostengr.HomeAutomation.Features.Transcripts.DomainServices.Interfaces
{
    public interface ISeleniumService  : IBaseService
    {
        IWebDriver StartBrowser();
        void CloseBrowser(IWebDriver driver);
    }
}