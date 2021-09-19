using System.Collections.Generic;
using Almostengr.TranscriptCleaner.Api.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.TranscriptCleaner.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class TranscriptController : ControllerBase
    {
        private readonly ILogger<TranscriptController> _logger;

        public TranscriptController(ILogger<TranscriptController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<TranscriptOutputDto> Post(TranscriptInputDto inputDto)
        {
            if (string.IsNullOrEmpty(inputDto.Input))
                return BadRequest("Input is required");

            string[] inputLines = inputDto.Input.Split('\n');

            if (inputLines[0].StartsWith("1") == false && inputLines[1].StartsWith("0:") == false)
                return BadRequest("Input is not in the correct format");

            List<string> blogOutput = new List<string>();
            List<string> videoOutput = new List<string>();
            int counter = 0;

            string videoString = string.Empty;
            string blogString = string.Empty;

            foreach (var line in inputLines)
            {
                counter = counter >= 4 ? 1 : counter + 1;

                string cleanedLine = line.ToUpper()
                        .Replace("UM", string.Empty)
                        .Replace("UH", string.Empty)
                        .Replace("  ", " ");

                videoString += cleanedLine + "\n";

                if (counter == 3)
                    blogString += cleanedLine + "\n";
            }

            TranscriptOutputDto outputDto = new TranscriptOutputDto();
            outputDto.VideoText = videoString;
            outputDto.BlogText = blogString;

            return Ok(outputDto);
        }

    }
}