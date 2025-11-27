using Almostengr.HomeAutomation.Features.Workers;

internal static class TranscriptExtensions
{
    public static void AddTranscriptServices(this IServiceCollection services)
    {
        services.AddHostedService<TranscriptWorker>();
    }
}