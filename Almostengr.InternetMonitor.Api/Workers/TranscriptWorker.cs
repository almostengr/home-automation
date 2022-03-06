using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Constants;
using Almostengr.InternetMonitor.Api.DataTransferObjects;
using Almostengr.InternetMonitor.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Api.Workers
{
    public class TranscriptWorker : BackgroundService
    {
        private readonly ITranscriptService _transcriptService;
        private readonly ITextFileService _textFileService;
        private readonly ILogger<TranscriptWorker> _logger;

        public TranscriptWorker(ITranscriptService transcriptService, ITextFileService textFileService,
            ILogger<TranscriptWorker> logger) : base()
        {
            _transcriptService = transcriptService;
            _textFileService = textFileService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var srtTranscripts = _transcriptService.GetTranscriptList(FileExtension.Srt);

                foreach (var srtTranscript in srtTranscripts)
                {
                    _logger.LogInformation($"TranscriptWorker: Processing {srtTranscript}");
                    var fileContent = _textFileService.GetFileContents(srtTranscript);

                    var transcriptOutput = _transcriptService.CleanTranscript(new TranscriptInputDto
                    {
                        Input = fileContent,
                        // VideoTitle = srtTranscript.Replace(TranscriptExt.Srt, string.Empty)
                        VideoTitle = Path.GetFileName(srtTranscript)
                    });

                    if (transcriptOutput != null)
                    {
                        _textFileService.DeleteFile(srtTranscript);
                    }
                    
                    _logger.LogInformation($"TranscriptWorker: Finished processing {srtTranscript}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}