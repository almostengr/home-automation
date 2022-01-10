namespace Almostengr.InternetMonitor.Api.DataTransfer
{
    public class HaApiResponseDto : BaseDto
    {
        public string Entity_Id { get; set; }
        public string State { get; set; }
        public string Last_Updated { get; set; }
    }
}