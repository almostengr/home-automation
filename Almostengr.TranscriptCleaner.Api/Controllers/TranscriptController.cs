using Almostengr.TranscriptCleaner.Api.DataTransferObjects;
using Almostengr.TranscriptCleaner.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.TranscriptCleaner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptController : ControllerBase
    {
        private readonly ILogger<TranscriptController> _logger;
        private readonly ITranscriptService _service;

        public TranscriptController(ILogger<TranscriptController> logger, ITranscriptService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public ActionResult<TranscriptOutputDto> CleanTranscript(TranscriptInputDto inputDto)
        {
            if (string.IsNullOrEmpty(inputDto.Input))
            {
                _logger.LogError("Input is empty");
                return BadRequest("Input is required");
            }

            string[] inputLines = inputDto.Input.Split('\n');

            if (inputLines[0].StartsWith("1") == false || inputLines[1].StartsWith("00:") == false)
            {
                _logger.LogError("Input is not in the correct format");
                return BadRequest("Input is not in the correct format");
            }

            return _service.CleanTranscript(inputDto);
        }

    }
}