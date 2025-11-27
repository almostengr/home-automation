namespace Almostengr.LcdDashboard.Models;

public interface IQuerySystemSettingService
{
    Task<IList<SystemSettingEntity>> GetListAsync();
    Task<SystemSettingEntity> GetByOptionAsync(int id);
}
