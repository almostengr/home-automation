using System;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.InternetMonitor.Api.Services;
using Microsoft.Extensions.Hosting;

namespace Almostengr.InternetMonitor.Api.Workers
{
    public class TranscriptWorker : BackgroundService
    {
        private readonly ITranscriptService _transcriptService;

        public TranscriptWorker(ITranscriptService transcriptService) : base()
        {
            _transcriptService = transcriptService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        }
    }
}