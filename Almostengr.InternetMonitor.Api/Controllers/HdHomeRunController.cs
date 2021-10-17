using Almostengr.InternetMonitor.Api.DataTransfer;
using Almostengr.InternetMonitor.Api.SeleniumAutomations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HdHomeRunController : ControllerBase
    {
        private readonly ILogger<HdHomeRunController> _logger;
        private readonly IHdHomeRunAutomation _hdHomeRunAutomation;

        public HdHomeRunController(ILogger<HdHomeRunController> logger, IHdHomeRunAutomation hdHomeRunAutomation)
        {
            _logger = logger;
            _hdHomeRunAutomation = hdHomeRunAutomation;
        }

        [HttpGet("update")]
        public IActionResult PendingUpdate()
        {
            var response = _hdHomeRunAutomation.IsUpdatePending();
            return Ok(new SensorState(response.ToString()));
        }

        [HttpPost("update")]
        public IActionResult PerformUpdate()
        {
            _hdHomeRunAutomation.PerformUpdate();
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