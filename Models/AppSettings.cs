namespace Almostengr.InternetMonitor.Model
{
    public class AppSettings
    {
        public Application Application { get; set; }
    }

    public class Application
    {
        public Router Router { get; set; }
        public Modem Modem { get; set; }
    }

    public class Router
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int MinWirelessClientCount { get; set; }
        public int RebootWait { get; set; }
    }

    public class Modem
    {
        public string Url { get; set; }
    }
}
