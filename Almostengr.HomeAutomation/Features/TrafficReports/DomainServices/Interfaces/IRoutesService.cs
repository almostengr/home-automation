namespace Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;

public interface IRoutesService
{
    Task<TimeSpan?> GetTravelTimeInTraffic(string originAddress, string destinationAddress);
    Task ExecuteAsync();
}