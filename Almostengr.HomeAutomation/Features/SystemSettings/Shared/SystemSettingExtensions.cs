using Almostengr.HomeAutomation.Features.SystemSettings.DomainServices;
using Almostengr.LcdDashboard.Features.SystemSettings.DomainServices;
using Almostengr.LcdDashboard.Features.SystemSettings.DomainServices.Interfaces;
using Almostengr.LcdDashboard.Models;

namespace Almostengr.LcdDashboard.Features.SystemSettings.Shared;

internal static class SystemSettingExtensions
{
    public static void AddSystemSettingServices (this IServiceCollection services)
    {
        services.AddTransient<IAddSystemSettingsService, AddSystemSettingService>();
        services.AddTransient<IQuerySystemSettingService, QuerySystemSettingService>();
        services.AddTransient<IUpdateSystemSettingsService, UpdateSystemSettingService>(); 
    }
}
