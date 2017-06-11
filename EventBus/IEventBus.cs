using EventBus.Handlers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IEventBus
    {
        void Register<TEventData>(Type eventHandler);
        void UnRegister<TEventData>(Type eventHandler);
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;
    }
}
