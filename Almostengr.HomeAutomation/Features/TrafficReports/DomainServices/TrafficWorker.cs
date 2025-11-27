namespace Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;

internal sealed class TrafficReportWorker : BackgroundService
{
    private readonly IRoutesService _routesService;

    public TrafficReportWorker(
        IRoutesService routesService
    )
    {
        _routesService = routesService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime currentTime = DateTime.Now;

            if (currentTime.DayOfWeek >= DayOfWeek.Monday &&
                currentTime.DayOfWeek <= DayOfWeek.Friday &&
                currentTime.Hour >= 5 &&
                currentTime.Hour <= 7)
            {
                await _routesService.ExecuteAsync();
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}