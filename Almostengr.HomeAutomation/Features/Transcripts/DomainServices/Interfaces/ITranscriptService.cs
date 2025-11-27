using Almostengr.HomeAutomation.Features.Common;
using Almostengr.HomeAutomation.Features.DataTransferObjects;

namespace Almostengr.HomeAutomation.Features.Services
{
    public interface ITranscriptService : IBaseService
    {
        TranscriptOutputDto CleanTranscript(TranscriptInputDto inputDto);
        bool IsValidTranscript(TranscriptInputDto inputDto);
        string[] GetTranscriptList(string srt);
    }
}
