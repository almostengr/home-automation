using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Almostengr.LcdDashboard.Features.SystemSettings.DomainServices;

internal sealed class QuerySystemSettingService : IQuerySystemSettingService
{
    private readonly ApplicationDbContext _dbContext;

    public QuerySystemSettingService(
        ApplicationDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<IList<SystemSettingEntity>> GetListAsync()
    {
        return await _dbContext.SystemSettings.ToListAsync();
    }

    public async Task<SystemSettingEntity> GetByOptionAsync(int id)
    {
        return await _dbContext.SystemSettings.SingleOrDefaultAsync(s => s.Id == id);
    }
}
