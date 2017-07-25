using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EventBus.Handlers;

namespace EventBus.EventStore
{
    public class InMemoryEventStore : IEventStore
    {
        /// <summary>
        /// 定义锁对象
        /// </summary>
        private static readonly object LockObj = new object();

        private readonly ConcurrentDictionary<Type, List<Type>> _eventAndHandlerMapping;

        public InMemoryEventStore()
        {
            _eventAndHandlerMapping = new ConcurrentDictionary<Type, List<Type>>();
        }
        public void AddRegister<T, TH>() where T : IEventData where TH : IEventHandler
        {
            AddRegister(typeof(T), typeof(TH));
        }


        public void AddActionRegister<T>(Action<T> action) where T : IEventData
        {
            var actionHandler = new ActionEventHandler<T>(action);

            AddRegister(typeof(T), actionHandler.GetType());
        }

        public void AddRegister(Type eventData, Type eventHandler)
        {
            lock (LockObj)
            {
                if (!HasRegisterForEvent(eventData))
                {
                    var handlers = new List<Type>();
                    _eventAndHandlerMapping.TryAdd(eventData, handlers);
                }

                if (_eventAndHandlerMapping[eventData].All(h => h != eventHandler))
                {
                    _eventAndHandlerMapping[eventData].Add(eventHandler);
                }
            }
        }

        public void RemoveRegister<T, TH>() where T : IEventData where TH : IEventHandler
        {
            var handlerToRemove = FindRegisterToRemove(typeof(T), typeof(TH));
            RemoveRegister(typeof(T), handlerToRemove);
        }

        public void RemoveActionRegister<T>(Action<T> action) where T : IEventData
        {
            var actionHandler = new ActionEventHandler<T>(action);
            var handlerToRemove = FindRegisterToRemove(typeof(T), actionHandler.GetType());
            RemoveRegister(typeof(T), handlerToRemove);
        }

        public void RemoveRegister(Type eventData, Type eventHandler)
        {
            if (eventHandler != null)
            {
                lock (LockObj)
                {
                    _eventAndHandlerMapping[eventData].Remove(eventHandler);
                    if (!_eventAndHandlerMapping[eventData].Any())
                    {
                        List<Type> removedHandlers;
                        _eventAndHandlerMapping.TryRemove(eventData, out removedHandlers);
                    }
                }
            }
        }

        private Type FindRegisterToRemove(Type eventData, Type eventHandler)
        {
            if (!HasRegisterForEvent(eventData))
            {
                return null;
            }

            return _eventAndHandlerMapping[eventData].FirstOrDefault(eh => eh == eventHandler);
        }

        public bool HasRegisterForEvent<T>() where T : IEventData
        {
            return _eventAndHandlerMapping.ContainsKey(typeof(T));
        }

        public bool HasRegisterForEvent(Type eventData)
        {
            return _eventAndHandlerMapping.ContainsKey(eventData);
        }

        public IEnumerable<Type> GetHandlersForEvent<T>() where T : IEventData
        {
            return GetHandlersForEvent(typeof(T));
        }

        public IEnumerable<Type> GetHandlersForEvent(Type eventData)
        {
            if (HasRegisterForEvent(eventData))
            {
                return _eventAndHandlerMapping[eventData];
            }

            return new List<Type>();
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _eventAndHandlerMapping.Keys.FirstOrDefault(eh => eh.Name == eventName);
        }

        public bool IsEmpty => !_eventAndHandlerMapping.Keys.Any();

        public void Clear() => _eventAndHandlerMapping.Clear();
    }
}