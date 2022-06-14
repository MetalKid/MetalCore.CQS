using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.PubSub
{
    public class Publisher : IPublisher
    {
        private readonly Func<Type, ICollection<dynamic>> _getSubscribersCallback;

        public Publisher(Func<Type, ICollection<dynamic>> getSubscribersCallback) =>
            _getSubscribersCallback = getSubscribersCallback;

        public Task PublishAsync<T>(T @event, CancellationToken token = default)
            where T : IEvent
        {
            Type type = typeof(ISubscriber<>).MakeGenericType(typeof(T));

            var subscribers = _getSubscribersCallback(type);
            if (subscribers == null || !subscribers.Any())
            {
                return Task.CompletedTask;
            }

            var tasks = new List<Func<Task>>();
            foreach (var subscriber in subscribers)
            {
                tasks.Add(() => subscriber.ExecuteEventAsync(@event, token));
            }

            return Task.WhenAll(tasks.AsParallel().Select(t => t()));
        }

        public void PublishAndForgetAsync<T>(T @event, CancellationToken token = default)
            where T : IEvent
        {
            Type type = typeof(ISubscriber<>).MakeGenericType(typeof(T));

            var subscribers = _getSubscribersCallback(type);
            if (subscribers == null || !subscribers.Any())
            {
                return;
            }

            foreach (var subscriber in subscribers)
            {
                Task.Run(() => subscriber.ExecuteEventAsync(@event, token));
            }
        }
    }
}
