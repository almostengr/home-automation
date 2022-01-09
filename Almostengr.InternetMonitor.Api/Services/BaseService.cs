using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public abstract class BaseService : IBaseService
    {
        private readonly ILogger<BaseService> _logger;
        private readonly AppSettings _appSettings;
        internal string RouterUrl = "http://router/";

        public BaseService(ILogger<BaseService> logger, AppSettings appSettings){
            _logger = logger;
            _appSettings = appSettings;
        }


        internal string SetUrlWithCredentials()
        {
            _logger.LogInformation("Converting router URL");

            if (string.IsNullOrEmpty(_appSettings.Router.Username) == false &&
                string.IsNullOrEmpty(_appSettings.Router.Password) == false)
            {
                string protocol = _appSettings.Router.Host.Substring(0, _appSettings.Router.Host.IndexOf("://"));
                string cleanedUrl = _appSettings.Router.Host.Replace("https://", "").Replace("http://", "");

                return protocol + "://" +
                    _appSettings.Router.Username + ":" +
                    _appSettings.Router.Password + "@" +
                    cleanedUrl;
            }

            return _appSettings.Router.Host;
        }

        
    }
}