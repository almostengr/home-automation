namespace Almostengr.LcdDashboard.Models;

public class TrafficReportViewModel
{
    public string StartingAddress { get; set; }
    public string EndingAddress { get; set; }
    public bool IsActive { get; set; }
    public int Id { get; set; }
    public string ShortName { get; set; }
}