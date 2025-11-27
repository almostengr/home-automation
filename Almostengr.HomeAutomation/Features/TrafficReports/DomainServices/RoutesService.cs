using Almostengr.HomeAutomation.Data;
using Almostengr.LcdDashboard.Features.TrafficReports.DomainServices;
using Google.Api.Gax.Grpc;
using Google.Maps.Routing.V2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Almostengr.HomeAutomation.Features.TrafficReports.DomainServices;

public class RoutesService : IRoutesService
{
    // Important: The API key is typically managed outside of the client constructor
    // using Application Default Credentials (ADC) or explicit CallSettings.
    // For simplicity here, we'll demonstrate using CallSettings with an API key.
    private readonly string _apiKey;
    private readonly ApplicationDbContext _dbContext;
    private readonly IQueryTrafficReportService _queryTrafficReportServices;

    public RoutesService(
        ApplicationDbContext dbContext,
        IQueryTrafficReportService queryTrafficReportServices,
        string apiKey)
    {
        _apiKey = apiKey;
        _dbContext = dbContext;
        _queryTrafficReportServices = queryTrafficReportServices;
    }

    /// <summary>
    /// Calculates the estimated travel time between two locations, considering real-time traffic.
    /// </summary>
    /// <param name="originAddress">The starting address (e.g., "10 Downing St, London").</param>
    /// <param name="destinationAddress">The ending address (e.g., "Buckingham Palace, London").</param>
    /// <returns>The duration in traffic as a TimeSpan, or null if the route could not be calculated.</returns>
    public async Task<TimeSpan?> GetTravelTimeInTraffic(string originAddress, string destinationAddress)
    {
        // 1. Initialize the Client
        // Note: Routes API uses gRPC by default.
        var client = await RoutesClient.CreateAsync();

        // 2. Define the Field Mask (Crucial for billing and performance)
        // We only request the duration with traffic. This field triggers the 'Routes API - Compute Routes - Advanced' SKU.
        const string fieldMask = "routes.duration,routes.travelAdvisory.durationWithTraffic";

        // Use CallSettings to pass the API Key and Field Mask in the gRPC headers
        var callSettings = CallSettings.FromHeader("X-Goog-Api-Key", _apiKey)
                                     .WithHeader("X-Goog-FieldMask", fieldMask);

        // 3. Construct the Request
        var request = new ComputeRoutesRequest
        {
            Origin = new Waypoint { Address = originAddress },
            Destination = new Waypoint { Address = destinationAddress },

            // Set travel mode to driving
            TravelMode = RouteTravelMode.Drive,

            // Critical for traffic: Request traffic-aware routing
            RoutingPreference = RoutingPreference.TrafficAware,

            // Critical for traffic: Use current time as departure time
            DepartureTime = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        try
        {
            // 4. Execute the API Call
            var response = await client.ComputeRoutesAsync(request, callSettings);

            // 5. Parse the Response
            if (response?.Routes != null && response.Routes.Count > 0)
            {
                var route = response.Routes[0];

                // The DurationWithTraffic object is where the traffic-aware time is returned.
                // var durationInTraffic = route.TravelAdvisory?.DurationWithTraffic;

                // if (durationInTraffic != null)
                // {
                //     // durationInTraffic.Seconds is a long, convert to TimeSpan
                //     return TimeSpan.FromSeconds(durationInTraffic.Seconds);
                // }

                var trafficAdjustedDuration = route.Duration;
                if (trafficAdjustedDuration != null)
                {
                    // Convert the Protobuf Duration (in seconds) to a .NET TimeSpan
                    long secondsInTraffic = trafficAdjustedDuration.Seconds;
                    return TimeSpan.FromSeconds(secondsInTraffic);
                }

                // Fallback to basic duration if traffic is not available/returned
                return TimeSpan.FromSeconds(route.Duration.Seconds);
            }
        }
        catch (RpcException ex)
        {
            // Handle API errors, such as invalid key, quota exceeded, or routing failure
            Console.WriteLine($"Routes API Error: {ex.Status.Detail}");
            // In a production app, you would log this error and handle it gracefully
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

        return null;
    }

    public async Task ExecuteAsync()
    {
        var reports = await _queryTrafficReportServices.GetListActiveAsync();
        foreach (var report in reports)
        {
            try
            {
                var result = await GetTravelTimeInTraffic(report.StartingAddress, report.EndingAddress);
                double seconds = result.HasValue ? result.Value.TotalSeconds : 0;
                var entityResult = report.UpdateTime((int)seconds);
                if (entityResult.Failed)
                {
                    /// todo  - add logging
                    continue;
                }

                _dbContext.Update(entityResult.Value);
            }
            catch (Exception)
            {
                /// todo log
                continue;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
