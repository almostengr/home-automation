using Almostengr.Common.DomainServices.Results;
using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Controllers;
using Almostengr.LcdDashboard.Features.SystemSettings.DomainServices.Interfaces;
using Almostengr.LcdDashboard.Models;

namespace Almostengr.HomeAutomation.Features.SystemSettings.DomainServices;

internal sealed class UpdateSystemSettingService : IUpdateSystemSettingsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IQuerySystemSettingService _queryService;

    public UpdateSystemSettingService(
        ApplicationDbContext dbContext,
        IQuerySystemSettingService queryService
    )
    {
        _dbContext = dbContext;
        _queryService = queryService;
    }

    public async Task<Result<SystemSettingViewModel>> ExecuteAsync(SystemSettingViewModel model)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await _queryService.GetByOptionAsync((int)model.Option);
            if (entity == null)
            {
                return new NotFoundResult<SystemSettingViewModel>();
            }

            var result = entity.Update(model.Value);
            if (result.Failed)
            {
                return Result<SystemSettingViewModel>.Failure(result.Errors);
            }

            _dbContext.Update(result.Value);
            await _dbContext.SaveChangesAsync();

            return Result<SystemSettingViewModel>.Success(model);
        }
        catch (Exception ex)
        {
            return Result<SystemSettingViewModel>.Failure(ex.Message);
        }
    }
}
