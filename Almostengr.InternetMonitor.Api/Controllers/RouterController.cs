using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Controllers
{
    public class RouterController : BaseController
    {
        private readonly ILogger<RouterController> _logger;
        private readonly IDdWrtRouterService _ddWrtRouterService;

        public RouterController(ILogger<RouterController> logger, IDdWrtRouterService DdWrtRouterService)
        {
            _logger = logger;
            _ddWrtRouterService = DdWrtRouterService;
        }

        [HttpPost("rebootrouter")]
        public async Task<IActionResult> RebootRouter()
        {
            await Task.Run(() => _ddWrtRouterService.RebootRouterAsync(null)).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("wifidevices")]
        public async Task<IActionResult> WifiDevices()
        {
            await Task.Run(() => _ddWrtRouterService.AreWifiDevicesConnectedAsync().ConfigureAwait(false));
            return Ok();
        }

        [HttpGet("uptime")]
        public IActionResult Uptime()
        {
            var response = _ddWrtRouterService.GetUpTime();
            return Ok(response);
        }
    }
}