using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.SeleniumAutomations
{
    public interface IBaseAutomation
    {
        IWebDriver StartBrowser();
        void CloseBrowser(IWebDriver driver);
    }
}