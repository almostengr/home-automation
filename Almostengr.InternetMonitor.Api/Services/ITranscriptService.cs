using Almostengr.InternetMonitor.Api.DataTransferObjects;

namespace Almostengr.InternetMonitor.Api.Services
{
    public interface ITranscriptService : IBaseService
    {
        TranscriptOutputDto CleanTranscript(TranscriptInputDto inputDto);
        bool IsValidTranscript(TranscriptInputDto inputDto);
        string[] GetTranscriptList(string srt);
    }
}
