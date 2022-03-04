using Almostengr.InternetMonitor.Api.DataTransferObjects;

namespace Almostengr.InternetMonitor.Api.Services
{
    public interface ITranscriptService
    {
        TranscriptOutputDto CleanTranscript(TranscriptInputDto inputDto);
        bool IsValidTranscript(TranscriptInputDto inputDto);
    }
}
