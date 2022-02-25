using Almostengr.TranscriptCleaner.Api.Common;
using Almostengr.TranscriptCleaner.Api.DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace Almostengr.TranscriptCleaner.Api.Services
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
            return outputDto;
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