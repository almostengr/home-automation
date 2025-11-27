using Almostengr.Common.DomainServices.Results;
using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Features.TrafficReports.Domain;
using Almostengr.LcdDashboard.Features.TrafficReports.Shared;
using Almostengr.LcdDashboard.Models;

namespace Almostengr.HomeAutomation.Features.TrafficReports.DomainServices;

internal sealed class AddTrafficReportServices : IAddTrafficReportServices
{
    private readonly ApplicationDbContext _dbContext;

    public AddTrafficReportServices(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<TrafficReportViewModel>> ExecuteAsync(TrafficReportViewModel model)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            Result<TrafficReportEntity> result = TrafficReportEntity.Create(model.StartingAddress, model.EndingAddress, model.IsActive, model.ShortName);
            if (result.Failed)
            {
                return Result<TrafficReportViewModel>.Failure(result.Errors); 
            }

            await _dbContext.TrafficReports.AddAsync(result.Value); 
            await _dbContext.SaveChangesAsync();
            return Result<TrafficReportViewModel>.Success(model);
        }
        catch(Exception ex)
        {
            return Result<TrafficReportViewModel>.Failure(ex.Message);
        }
    }
}
