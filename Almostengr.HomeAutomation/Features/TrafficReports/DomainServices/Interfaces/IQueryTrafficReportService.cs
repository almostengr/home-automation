using Almostengr.LcdDashboard.Features.TrafficReports.Domain;

namespace Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;

public interface IQueryTrafficReportService
{
    Task<TrafficReportEntity> GetByIdAsync(int id);
    Task<IList<TrafficReportEntity>> GetListActiveAsync();
}