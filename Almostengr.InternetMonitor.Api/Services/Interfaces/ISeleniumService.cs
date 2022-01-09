using OpenQA.Selenium;

namespace Almostengr.InternetMonitor.Api.Services
{
    public interface ISeleniumService  : IBaseService
    {

        IWebDriver StartBrowser();
        void CloseBrowser(IWebDriver driver);
    }
}