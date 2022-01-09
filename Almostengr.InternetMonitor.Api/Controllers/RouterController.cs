using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouterController : ControllerBase
    {
        private readonly ILogger<RouterController> _logger;
        private readonly IDdWrtRouterService _ddWrtRouterAutomation;

        public RouterController(ILogger<RouterController> logger, IDdWrtRouterService ddWrtRouterAutomation){
            _logger = logger;
            _ddWrtRouterAutomation = ddWrtRouterAutomation;
        }

        [HttpPost("rebootrouter")]
        public async Task<IActionResult> RebootRouter()
        {
            await Task.Run(() => _ddWrtRouterAutomation.RebootRouterAsync(null)).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("wifidevices")]
        public async Task<IActionResult> WifiDevices()
        {
            await Task.Run(() => _ddWrtRouterAutomation.AreWifiDevicesConnectedAsync().ConfigureAwait(false));
            return Ok();
        }

        [HttpGet("uptime")]
        public IActionResult Uptime()
        {
            var response = _ddWrtRouterAutomation.GetUpTime();
            return Ok(response);
        }
    }
}