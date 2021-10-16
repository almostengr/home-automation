using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Almostengr.InternetMonitor.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _httpClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8051/");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking Wifi Devices {time}", DateTimeOffset.Now);
                
                await _httpClient.GetAsync("api/router/wifidevices").ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        public override void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
