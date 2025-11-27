using Almostengr.HomeAutomation.Features.Common;

namespace Almostengr.HomeAutomation.Features.DataTransfer
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
