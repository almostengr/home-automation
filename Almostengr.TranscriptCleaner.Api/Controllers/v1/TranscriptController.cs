using Almostengr.TranscriptCleaner.Api.Common;
using Almostengr.TranscriptCleaner.Api.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Almostengr.TranscriptCleaner.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TranscriptController : ControllerBase
    {
        private readonly ILogger<TranscriptController> _logger;

        public TranscriptController(ILogger<TranscriptController> logger)
        {
            _logger = logger;
        }

        [HttpPost("clean")]
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

            int counter = 0;
            string videoString = string.Empty;
            string blogString = string.Empty;

            foreach (var line in inputLines)
            {
                counter = counter >= 4 ? 1 : counter + 1;

                string cleanedLine = line.ToUpper()
                    .Replace("UM", string.Empty)
                    .Replace("UH", string.Empty)
                    .Replace("[MUSIC] YOU", "[MUSIC]")
                    .Replace("  ", " ")
                    .Trim();

                videoString += cleanedLine + Constants.NewLine;

                if (counter == 3)
                {
                    blogString += cleanedLine + Constants.Space;
                }
            }

            blogString = RemoveDupesFromBlogString(blogString);
            blogString = CleanBlogString(blogString);

            TranscriptOutputDto outputDto = new TranscriptOutputDto();
            outputDto.VideoText = videoString;
            outputDto.BlogText = blogString;
            outputDto.BlogWords = blogString.Split(' ').Length;

            _logger.LogInformation("Transcript processed successfully");
            return Ok(outputDto);
        }

        private string CleanBlogString(string blogText)
        {
            return blogText.Replace("[MUSIC]", string.Empty)
                .Replace("  ", " ")
                .Trim();
        }

        private string RemoveDupesFromBlogString(string blogText)
        {
            string[] words = blogText.Split(' ');
            string previousWord = string.Empty;
            string output = string.Empty;

            foreach (var word in words)
            {
                if (previousWord != word)
                {
                    output += word + Constants.Space;
                }
            }

            return output;
        }
    }
}