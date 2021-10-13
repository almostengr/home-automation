namespace Almostengr.InternetMonitor.Model
{
    public class AppSettings
    {
        public Router Router { get; set; }
        public Modem Modem { get; set; }
        public HomeAssistant HomeAssistant { get; set; }
    }

    public class HomeAssistant
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }

    public class Router
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int MinWirelessClientCount { get; set; }
        public int FailCount { get; set; }
    }

    public class Modem
    {
        public string Url { get; set; }
    }
}
