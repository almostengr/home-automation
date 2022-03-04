using System;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Services
{
    public class TextFileService : ITextFileService
    {
        private readonly ILogger<TextFileService> _logger;

        public TextFileService(ILogger<TextFileService> logger)
        {
            _logger = logger;
        }

        public string GetTextFileContent(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTextFileContent(string filePath, string content)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving text file content");
            }
        }
    }
}