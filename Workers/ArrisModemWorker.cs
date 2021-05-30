using Almostengr.InternetMonitor.Model;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Workers
{
    public class ArrisModemWorker : WorkerBase
    {
        private readonly ILogger<ArrisModemWorker> _logger;
        private readonly AppSettings _appSettings;

        public ArrisModemWorker(ILogger<ArrisModemWorker> logger, AppSettings appSettings) : base(logger, appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }
    }
}