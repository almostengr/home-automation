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

        [HttpPost]
        public async Task<IActionResult> RebootRouter()
        {
            await _ddWrtRouterAutomation.RebootRouterAsync().ConfigureAwait(false);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> WifiDevices()
        {
            _ddWrtRouterAutomation.AreWifiDevicesConnected();
            return Ok();
        }
    }
}