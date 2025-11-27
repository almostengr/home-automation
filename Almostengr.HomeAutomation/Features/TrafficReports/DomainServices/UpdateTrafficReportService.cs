using Almostengr.Common.DomainServices.Results;
using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Features.TrafficReports.Domain;
using Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;
using Almostengr.LcdDashboard.Models;

namespace Almostengr.HomeAutomation.Features.TrafficReports.DomainServices;

internal sealed class UpdateTrafficReportService : IUpdateTrafficReportService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IQuerySystemSettingService _querySystemSettingService;
    private readonly IQueryTrafficReportService _queryTrafficService;

    public UpdateTrafficReportService(
        ApplicationDbContext dbContext,
        IQuerySystemSettingService querySystemSettingService,
        IQueryTrafficReportService queryTrafficService
    )
    {
        _dbContext = dbContext;
        _querySystemSettingService = querySystemSettingService; 
        _queryTrafficService = queryTrafficService;
    }

    public async Task<Result<TrafficReportViewModel>> ExecuteAsync(TrafficReportViewModel model)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            TrafficReportEntity entity = await _queryTrafficService.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return new NotFoundResult<TrafficReportViewModel>();
            }

            var result = entity.Update(model.StartingAddress, model.EndingAddress, model.IsActive, model.ShortName);
            if (result.Failed)
            {
                return Result<TrafficReportViewModel>.Failure(result.Errors);
            }

            _dbContext.TrafficReports.Update(result.Value); 
            await _dbContext.SaveChangesAsync();

            return Result<TrafficReportViewModel>.Success(model);
        }   
        catch (Exception ex)
        {
            return Result<TrafficReportViewModel>.Failure(ex.Message) ;
        }
    }
}
