using Almostengr.HomeAutomation.Features.TrafficReports.DomainServices;
using Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;

namespace Almostengr.LcdDashboard.Features.TrafficReports.Shared;

internal static class TrafficReportExtensions
{
    public static void AddTrafficReportServices(this IServiceCollection services)
    {
        services.AddTransient<IAddTrafficReportServices, AddTrafficReportServices>();
        services.AddTransient<IQueryTrafficReportService, QueryTrafficReportServices>();
        services.AddTransient<IRoutesService, RoutesService>();
        services.AddTransient<IUpdateTrafficReportService, UpdateTrafficReportService>();
        services.AddHostedService<TrafficReportWorker>();
    }
}
