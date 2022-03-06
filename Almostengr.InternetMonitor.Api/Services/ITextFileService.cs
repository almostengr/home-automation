namespace Almostengr.InternetMonitor.Api.Services
{
    public interface ITextFileService : IBaseService
    {
        string GetFileContents(string filePath);
        void SaveFileContents(string filePath, string content);
        void DeleteFile(string filePath);
    }
}