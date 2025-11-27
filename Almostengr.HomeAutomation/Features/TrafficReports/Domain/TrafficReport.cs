using System.ComponentModel.DataAnnotations;
using Almostengr.Common.DomainServices.Results;
using Google.Type;

namespace Almostengr.LcdDashboard.Features.TrafficReports.Domain;

public class TrafficReportEntity
{
    [Key]
    public int Id { get; private set; }

    [Required, StringLength(250)]
    public string StartingAddress { get; private set; }

    [Required, StringLength(250)]
    public string EndingAddress { get; private set; }

    public bool IsActive { get; private set; }
    public long TravelTimeSeconds { get; private set; }
    public System.DateTime ModifiedDate { get; private set; }

    [Required, StringLength(5)]
    public string ShortName { get; private set; }

    internal static Result<TrafficReportEntity> Create(string startingAddress, string endingAddress, bool isActive, string shortName)
    {
        TrafficReportEntity report = new();
        return report.Update(startingAddress, endingAddress, isActive, shortName);
    }

    internal Result<TrafficReportEntity> Update(string startingAddress, string endingAddress, bool isActive, string shortName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startingAddress, nameof(startingAddress));
        ArgumentException.ThrowIfNullOrWhiteSpace(endingAddress, nameof(endingAddress));

        StartingAddress = startingAddress;
        EndingAddress = endingAddress;
        IsActive = isActive;
        TravelTimeSeconds = 0;
        ShortName = shortName;
        ModifiedDate = System.DateTime.Now;
        return Result<TrafficReportEntity>.Success(this);
    }

    internal Result<TrafficReportEntity> UpdateTime(int seconds)
    {
        if (seconds < 0)
        {
            return Result<TrafficReportEntity>.Failure("Invalid seconds");
        }

        TravelTimeSeconds = seconds;
        return Result<TrafficReportEntity>.Success(this);
    }
}
