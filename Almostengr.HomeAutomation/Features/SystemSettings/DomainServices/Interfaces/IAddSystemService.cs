using Almostengr.Common.DomainServices.Results;
using Almostengr.LcdDashboard.Controllers;

namespace Almostengr.LcdDashboard.Models;

public interface IAddSystemSettingsService
{
    Task<Result<SystemSettingViewModel>> ExecuteAsync(SystemSettingViewModel model);
}
