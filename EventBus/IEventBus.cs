using EventBus.Handlers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IEventBus
    {
        void Register<TEventData>(IEventHandler eventHandler);

        void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        void Register(Type eventType, IEventHandler handler);

        void UnRegister<TEventData>(Type eventHandler);

        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;
    }
}
