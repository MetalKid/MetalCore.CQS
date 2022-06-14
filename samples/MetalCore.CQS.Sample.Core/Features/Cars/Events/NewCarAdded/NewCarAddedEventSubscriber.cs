using MetalCore.CQS.PubSub;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Events.NewCarAdded
{
    public class NewCarAddedEventSubscriber : ISubscriber<NewCarAddedEvent>
    {
        public async Task ExecuteEventAsync(NewCarAddedEvent request, CancellationToken token = default)
        {
            await Task.Delay(1000);

            Console.WriteLine("New car was added event!");
        }
    }
}
