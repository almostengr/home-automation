using System.IO;
using Almostengr.InternetMonitor.Api.Constants;
using Almostengr.InternetMonitor.Api.DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public class SrtTranscriptService : ITranscriptService
    {
        private readonly ILogger<SrtTranscriptService> _logger;
        private readonly ITextFileService _textFileService;

        public SrtTranscriptService(ILogger<SrtTranscriptService> logger, ITextFileService textFileService) 
        {
            _logger = logger;
            _textFileService = textFileService;
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

                string cleanedLine = line
                    .ToUpper()
                    .Replace("UM", string.Empty)
                    .Replace("UH", string.Empty)
                    .Replace("[MUSIC] YOU", "[MUSIC]")
                    .Replace("  ", " ")
                    .Replace("ALL RIGHT", "ALRIGHT")
                    .Trim();

                videoString += cleanedLine + Formatting.NewLine;

                if (counter == 3)
                {
                    blogString += cleanedLine + Formatting.NewLine;
                }
            }

            blogString = RemoveDupesFromBlogString(blogString);
            blogString = CleanBlogString(blogString);

            TranscriptOutputDto outputDto = new();
            outputDto.VideoTitle = inputDto.VideoTitle;
            outputDto.VideoText = videoString;
            outputDto.BlogText = blogString;
            outputDto.BlogWords = blogString.Split(' ').Length;

            _logger.LogInformation("Transcript processed successfully");

            _textFileService.SaveFileContents(
                $"{Transcript.OutputDirectory}/{outputDto.VideoTitle}.srt",
                outputDto.VideoText);

            _textFileService.SaveFileContents(
                $"{Transcript.OutputDirectory}/{outputDto.VideoTitle}.md",
                outputDto.BlogText);

            return outputDto;
        }

        public string[] GetTranscriptList(string srt)
        {
            if (Directory.Exists(Transcript.InputDirectory) == false)
            {
                Directory.CreateDirectory(Transcript.InputDirectory);
            }

            return Directory.GetFiles(Transcript.InputDirectory, $"*{FileExtension.Srt}");
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
            return blogText
                .Replace("  ", " ")
                .Replace("[MUSIC]", "(MUSIC)")
                .Replace("AND SO ", string.Empty)
                .Replace("FACEBOOK", "<a href=\"https://www.facebook.com/rhtservicesllc/\" target=\"_blank\">FACEBOOK</a>")
                .Replace("INSTAGRAM", "<a href=\"https://www.instagram.com/rhtservicesllc/\" target=\"_blank\">INSTAGRAM</a>")
                .Replace("RHTSERVICES.NET", "[RHTSERVICES.NET](/)")
                .Replace("YOUTUBE", "<a href=\"https://www.youtube.com/c/RobinsonHandyandTechnologyServices?sub_confirmation=1\" target=\"_blank\">YOUTUBE</a>")
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