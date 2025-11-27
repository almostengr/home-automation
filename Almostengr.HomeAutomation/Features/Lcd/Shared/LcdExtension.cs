using Almostengr.LcdDashboard.Features.Lcd.DomainServices;

namespace Almostengr.LcdDashboard.Features.Lcd.Shared;

public static class LcdExtension
{
    public static void AddLcdServices (this IServiceCollection services) { 
        services.AddHostedService<LcdWorker>();
    }
}