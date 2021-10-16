using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.SeleniumAutomations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RouterController : ControllerBase
    {
        private readonly ILogger<RouterController> _logger;
        private readonly IDdWrtRouterAutomation _ddWrtRouterAutomation;

        public RouterController(ILogger<RouterController> logger, IDdWrtRouterAutomation ddWrtRouterAutomation){
            _logger = logger;
            _ddWrtRouterAutomation = ddWrtRouterAutomation;
        }

        [HttpPost("rebootrouter")]
        public async Task<IActionResult> RebootRouter()
        {
            // await _ddWrtRouterAutomation.RebootRouterAsync(null).ConfigureAwait(false);
            await Task.Run(() => _ddWrtRouterAutomation.RebootRouterAsync(null)).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("wifidevices")]
        public async Task<IActionResult> WifiDevices()
        {
            // await _ddWrtRouterAutomation.AreWifiDevicesConnectedAsync().ConfigureAwait(false);
            await Task.Run(() => _ddWrtRouterAutomation.AreWifiDevicesConnectedAsync().ConfigureAwait(false));
            return Ok();
        }
    }
}