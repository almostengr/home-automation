using Almostengr.TranscriptCleaner.Api.DataTransferObjects;

namespace Almostengr.TranscriptCleaner.Api.Services
{
    public interface ITranscriptService
    {
        TranscriptOutputDto CleanTranscript(TranscriptInputDto inputDto);
    }
}
