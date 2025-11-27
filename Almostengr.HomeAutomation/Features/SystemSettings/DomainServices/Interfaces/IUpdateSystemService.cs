using Almostengr.Common.DomainServices.Results;
using Almostengr.LcdDashboard.Controllers;

namespace Almostengr.LcdDashboard.Features.SystemSettings.DomainServices.Interfaces;

public interface IUpdateSystemSettingsService
{
    Task<Result<SystemSettingViewModel>> ExecuteAsync(SystemSettingViewModel model);
}