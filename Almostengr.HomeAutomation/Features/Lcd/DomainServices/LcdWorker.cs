using Almostengr.LcdDashboard.Features.Lcd.Domain;
using Almostengr.LcdDashboard.Features.TrafficReports.Domain;
using Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;
using Iot.Device.CharacterLcd;

namespace Almostengr.LcdDashboard.Features.Lcd.DomainServices;

public partial class LcdWorker : BackgroundService
{
    private readonly Lcd1602 _lcdService;
    private readonly ILogger<LcdWorker> _logger;
    private readonly IQueryTrafficReportService _queryTrafficReportService;
    private const int SCREEN_SECONDS = 3;

    public LcdWorker(
        Lcd1602 lcdService,
        ILogger<LcdWorker> logger,
        IQueryTrafficReportService queryTrafficReportService)
    {
        _lcdService = lcdService;
        _logger = logger;
        _queryTrafficReportService = queryTrafficReportService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LcdScreenOption lcdScreen = LcdScreenOption.Version;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                switch (lcdScreen)
                {
                    case LcdScreenOption.Version:
                        UpdateDisplay("rhtservices.net");
                        lcdScreen++;
                        break;

                    // case LcdScreenOption.Traffic:
                    //     IList<TrafficReportEntity> reports = await _queryTrafficReportService.GetListActiveAsync();
                    //     foreach (var report in reports)
                    //     {
                    //         UpdateDisplay("Traffic", $"{report.ShortName} {TimeSpan.FromSeconds(report.TravelTimeSeconds).Minutes}");
                    //         await Task.Delay(TimeSpan.FromSeconds(SCREEN_SECONDS));
                    //     }
                    //     lcdScreen++;
                    //     break;

                    default:
                        DateTime currentTime = DateTime.Now;
                        UpdateDisplay(currentTime.Date.ToString(), currentTime.TimeOfDay.ToString());
                        lcdScreen = 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{message}", ex.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(SCREEN_SECONDS), stoppingToken);
        }
    }

    private void UpdateDisplay(string line1, string line2 = "")
    {
        _lcdService.Clear();
        _lcdService.SetCursorPosition(0, 0);
        _lcdService.Write(line1);
        _lcdService.SetCursorPosition(0, 1);
        _lcdService.Write(line2);
    }
}
