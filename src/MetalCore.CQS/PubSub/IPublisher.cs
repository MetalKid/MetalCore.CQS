using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.PubSub
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken token = default)
            where T : IEvent;

        void PublishAndForgetAsync<T>(T @event, CancellationToken token = default)
            where T : IEvent;
    }
}