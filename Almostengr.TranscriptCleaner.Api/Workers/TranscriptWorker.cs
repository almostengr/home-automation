using System.Threading;
using System.Threading.Tasks;
using Almostengr.TranscriptCleaner.Api.DataTransferObjects;
using Almostengr.TranscriptCleaner.Api.Services;
using Microsoft.Extensions.Hosting;

namespace Almostengr.TranscriptCleaner.Api.Workers
{
    public class TranscriptWorker : BackgroundService
    {
        private readonly ITranscriptService _service;

        public TranscriptWorker(ITranscriptService service)
        {
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                TranscriptInputDto inputDto = new TranscriptInputDto();
                
                _service.CleanTranscript(inputDto);

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}