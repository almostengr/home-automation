namespace Almostengr.InternetMonitor.Api.Services
{
    public interface ITextFileService : IBaseService
    {
        string GetTextFileContent(string filePath);
        void SaveTextFileContent(string filePath, string content);
    }
}