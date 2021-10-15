namespace Almostengr.InternetMonitor.Api.Models
{
    public class AppSettings
    {
        public HomeAssistant HomeAssistant { get; set; }
        public Router Router { get; set; }
    }

    public class Router
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class HomeAssistant
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }
}