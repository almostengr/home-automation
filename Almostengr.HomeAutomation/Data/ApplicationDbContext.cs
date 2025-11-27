using Almostengr.LcdDashboard.Features.TrafficReports.Domain;
using Almostengr.LcdDashboard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Almostengr.HomeAutomation.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<SystemSettingEntity> SystemSettings { get; set; }
    public DbSet<TrafficReportEntity> TrafficReports { get; set; }
}
