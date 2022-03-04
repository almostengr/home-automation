using Almostengr.InternetMonitor.Api.Constants;
using Almostengr.InternetMonitor.Api.DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public class TranscriptService : ITranscriptService
    {
        private readonly ILogger<TranscriptService> _logger;

        public TranscriptService(ILogger<TranscriptService> logger)
        {
            _logger = logger;
        }

        public TranscriptOutputDto CleanTranscript(TranscriptInputDto inputDto)
        {
            string[] inputLines = inputDto.Input.Split('\n');

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
                    .Replace("RHTSERVICES.NET", "[RHTSERVICES.NET](/)")
                    .Replace("INSTAGRAM", "<a href=\"https://www.instagram.com/rhtservicesllc/\">INSTAGRAM</a>")
                    .Replace("FACEBOOK", "<a href=\"https://www.facebook.com/rhtservicesllc/\">FACEBOOK</a>")
                    .Trim();

                videoString += cleanedLine + Formatting.NewLine;

                if (counter == 3)
                {
                    blogString += cleanedLine + Formatting.Space;
                }
            }

            blogString = RemoveDupesFromBlogString(blogString);
            blogString = CleanBlogString(blogString);

            TranscriptOutputDto outputDto = new TranscriptOutputDto();
            outputDto.VideoText = videoString;
            outputDto.BlogText = blogString;
            outputDto.BlogWords = blogString.Split(' ').Length;

            _logger.LogInformation("Transcript processed successfully");
            return outputDto;
        }

        public bool IsValidTranscript(TranscriptInputDto inputDto)
        {
            if (string.IsNullOrEmpty(inputDto.Input))
            {
                _logger.LogError("Input is empty");
                return false;
            }

            string[] inputLines = inputDto.Input.Split('\n');
            return (inputLines[0].StartsWith("1") == true && inputLines[1].StartsWith("00:") == true);
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
                    output += word + Formatting.Space;
                }
            }

            return output;
        }
    }
}