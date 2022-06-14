using MetalCore.CQS.PubSub;
using MetalCore.CQS.Sample.Core.DataStore;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Events
{
    public class NewCarAddedEvent : IEvent
    {
        public CarEntity CarAdded { get; }

        public NewCarAddedEvent(CarEntity car) =>
            CarAdded = car;
    }
}
