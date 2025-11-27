using Almostengr.HomeAutomation.Features.Common;

namespace Almostengr.HomeAutomation.Features.Services
{
    public interface ITextFileService : IBaseService
    {
        string GetFileContents(string filePath);
        void SaveFileContents(string filePath, string content);
        void DeleteFile(string filePath);
    }
}