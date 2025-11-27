using Almostengr.Common.DomainServices.Results;
using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Controllers;
using Almostengr.LcdDashboard.Models;

namespace Almostengr.HomeAutomation.Features.SystemSettings.DomainServices;

internal sealed class AddSystemSettingService : IAddSystemSettingsService
{
    private readonly ApplicationDbContext _dbContext;

    public AddSystemSettingService(
        ApplicationDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SystemSettingViewModel>> ExecuteAsync(SystemSettingViewModel model)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            var result = SystemSettingEntity.Create(model.Option, model.Value);
            if (result.Failed)
            {
                return Result<SystemSettingViewModel>.Failure(result.Errors);
            }

            await _dbContext.SystemSettings.AddAsync(result.Value);
            await _dbContext.SaveChangesAsync();

            return Result<SystemSettingViewModel>.Success(model);
        }
        catch (Exception ex)
        {
            return Result<SystemSettingViewModel>.Failure(ex.Message);
        }
    }
}
