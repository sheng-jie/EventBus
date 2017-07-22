using System;
using System.Collections.Generic;
using EventBus.Handlers;

namespace EventBus.EventStore
{
    public interface IEventStore
    {
        void AddRegister<T, TH>() where T : IEventData where TH : IEventHandler;
        void AddRegister(Type eventData, Type eventHandler);
        void AddActionRegister<T>(Action<T> action) where T : IEventData;
        void RemoveRegister<T, TH>() where T : IEventData where TH : IEventHandler;
        void RemoveActionRegister<T>(Action<T> action) where T : IEventData;
        void RemoveRegister(Type eventData, Type eventHandler);
        bool HasRegisterForEvent<T>() where T : IEventData;
        bool HasRegisterForEvent(Type eventData);
        IEnumerable<Type> GetHandlersForEvent<T>() where T : IEventData;
        IEnumerable<Type> GetHandlersForEvent(Type eventData);
        bool IsEmpty { get; }
        void Clear();
    }
}