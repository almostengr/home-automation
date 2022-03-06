namespace Almostengr.InternetMonitor.Api.DataTransfer
{
    public class SensorStateDto : BaseDto
    {
        public SensorStateDto(string state)
        {
            State = state;
        }

        public string State { get; set; }
    }
}
