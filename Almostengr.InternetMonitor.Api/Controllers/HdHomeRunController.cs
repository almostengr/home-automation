using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Almostengr.InternetMonitor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HdHomeRunController : ControllerBase
    {
        private readonly ILogger<HdHomeRunController> _logger;
        private readonly IHdHomeRunService _hdHomeRunAutomation;

        public HdHomeRunController(ILogger<HdHomeRunController> logger, IHdHomeRunService hdHomeRunAutomation)
        {
            _logger = logger;
            _hdHomeRunAutomation = hdHomeRunAutomation;
        }

        [HttpGet("update")]
        public async Task<IActionResult> PendingUpdate()
        {
            var response = await _hdHomeRunAutomation.IsUpdatePendingAsync();
            return Ok(new SensorState(response.ToString()));
        }

        [HttpPost("update")]
        public async Task<IActionResult> PerformUpdate()
        {
            await _hdHomeRunAutomation.PerformUpdateAsync();
            return Ok();
        }

        [HttpGet("status/system")]
        public IActionResult CheckDeviceOnline()
        {
            var response = _hdHomeRunAutomation.SystemStatus();
            return Ok(new SensorState(response.ToString()));
        }

        [HttpGet("status/tuner")]
        public IActionResult TunerStatus()
        {
            var response = _hdHomeRunAutomation.TunerStatus();
            return Ok(new SensorState(response.ToString()));
        }

    }
}