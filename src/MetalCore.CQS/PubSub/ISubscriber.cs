using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.PubSub
{
    public interface ISubscriber<T> 
        where T : IEvent
    {
        Task ExecuteEventAsync(T request, CancellationToken token = default);
    }
}