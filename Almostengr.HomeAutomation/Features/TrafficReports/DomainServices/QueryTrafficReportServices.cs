using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Features.TrafficReports.Domain;
using Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;
using Microsoft.EntityFrameworkCore;

namespace Almostengr.HomeAutomation.Features.TrafficReports.DomainServices;

internal sealed class QueryTrafficReportServices : IQueryTrafficReportService
{
    private readonly ApplicationDbContext _dbContext;

    public QueryTrafficReportServices(
        ApplicationDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<IList<TrafficReportEntity>> GetListActiveAsync()
    {
        return await _dbContext.TrafficReports
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<TrafficReportEntity> GetByIdAsync(int id)
    {
        return await _dbContext.TrafficReports.SingleOrDefaultAsync(r => r.Id == id);
    }
}
