using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.DataTransfer;
using Almostengr.InternetMonitor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Controllers
{
    public class HdHomeRunController : BaseController
    {
        private readonly ILogger<HdHomeRunController> _logger;
        private readonly IHdHomeRunService _hdHomeRunService;

        public HdHomeRunController(ILogger<HdHomeRunController> logger, IHdHomeRunService hdHomeRunService)
        {
            _logger = logger;
            _hdHomeRunService = hdHomeRunService;
        }

        [HttpGet("update")]
        public async Task<IActionResult> PendingUpdate()
        {
            var response = await _hdHomeRunService.IsUpdatePendingAsync();
            return Ok(new SensorStateDto(response.ToString()));
        }

        [HttpPost("update")]
        public async Task<IActionResult> PerformUpdate()
        {
            await _hdHomeRunService.PerformUpdateAsync();
            return Ok();
        }

        [HttpGet("status/system")]
        public IActionResult CheckDeviceOnline()
        {
            var response = _hdHomeRunService.SystemStatus();
            return Ok(new SensorStateDto(response.ToString()));
        }

        [HttpGet("status/tuner")]
        public IActionResult TunerStatus()
        {
            var response = _hdHomeRunService.TunerStatus();
            return Ok(new SensorStateDto(response.ToString()));
        }

    }
}