using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public abstract class BaseService : IBaseService
    {
        private readonly ILogger<BaseService> _logger;

        public BaseService(ILogger<BaseService> logger){
            _logger = logger;
        }
        
    }
}