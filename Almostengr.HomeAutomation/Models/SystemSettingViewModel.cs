using Almostengr.LcdDashboard.Features.SystemSettings.Domain;
using Almostengr.LcdDashboard.Models;

namespace Almostengr.LcdDashboard.Controllers;

public class SystemSettingViewModel
{
    public SystemSettingViewModel()
    {
    }

    public SystemSettingViewModel(SystemSettingEntity entity)
    {
        Option = entity.Option;
        Value = entity.Value;
    }

    public SystemSettingOption Option { get; set; }
    public string Value { get; set; }
    public List<SystemSettingOption> Options { get; set; }

    internal void AssignToEntity(SystemSettingEntity entity)
    {
        if (entity == null)
        {
            return;
        }


    }
}