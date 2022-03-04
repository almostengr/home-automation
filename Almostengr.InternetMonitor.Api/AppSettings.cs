namespace Almostengr.InternetMonitor.Api
{
    public class AppSettings
    {
        public HomeAssistant HomeAssistant { get; set; } = new();
        public Router Router { get; set; } = new();
    }

    public class Router
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int MinWirelessClientCount { get; set; } = 2;
    }

    public class HomeAssistant
    {
        public string Url { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}