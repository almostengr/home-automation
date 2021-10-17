using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces
{
    public interface IBaseAutomation
    {
        IWebDriver StartBrowser();
        void CloseBrowser(IWebDriver driver);
    }
}