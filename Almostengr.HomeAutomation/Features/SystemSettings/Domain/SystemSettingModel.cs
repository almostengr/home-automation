using System.ComponentModel.DataAnnotations;
using Almostengr.Common.DomainServices.Results;
using Almostengr.LcdDashboard.Features.SystemSettings.Domain;

namespace Almostengr.LcdDashboard.Models;

public class SystemSettingEntity
{
    private SystemSettingEntity(SystemSettingOption option)
    {
        Id = (int) option;
    }

    [Key]
    public int Id { get; private set; }

    [Required, StringLength(150)]
    public string Value { get; private set; }

    public SystemSettingOption Option => (SystemSettingOption)Id;

    public static Result<SystemSettingEntity> Create(SystemSettingOption option, string value)
    {
        SystemSettingEntity model = new(option);
        return model.Update(value);
    }

    public Result<SystemSettingEntity> Update(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<SystemSettingEntity>.Failure("Value is required.");
        }

        Value = value;
        return Result<SystemSettingEntity>.Success(this);
    }
}
