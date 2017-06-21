using EventBus.Handlers;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IEventBus
    {
        void Register<TEventData>(IEventHandler eventHandler);

        void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        void Register(Type eventType, Type handler);

        void RegisterAllEventHandlerFromAssembly(Assembly assembly);

        void UnRegister<TEventData>(Type handlerType) where TEventData : IEventData;

        void UnRegisterAll<TEventData>() where TEventData : IEventData;

        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;
        void Trigger<TEventData>(Type eventHandlerType, TEventData eventData) where TEventData : IEventData;

        Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData;

        Task TriggerAsycn<TEventData>(Type eventHandlerType, TEventData eventData) where TEventData : IEventData;

    }
}
