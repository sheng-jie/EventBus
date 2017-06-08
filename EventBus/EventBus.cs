using EventBus.Handlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace EventBus
{
    public class EventBus
    {
        public static EventBus Default => new EventBus();

        private readonly ConcurrentDictionary<Type, List<Type>> _handers;

        public EventBus()
        {
            MapEventToHandler();
            _handers = new ConcurrentDictionary<Type, List<Type>>();
        }

        private void MapEventToHandler()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IEventHandler).IsAssignableFrom(type))//判断当前类型是否实现了IEventHandler接口
                {
                    Type handlerInterface = type.GetInterface("IEventHandler`1");//获取该类实现的泛型接口
                    Type eventDataType = handlerInterface.GetGenericArguments()[0]; // 获取泛型接口指定的参数类型

                    if (_handers.ContainsKey(eventDataType))
                    {
                        List<Type> handlerTypes = _handers[eventDataType];
                        handlerTypes.Add(type);
                        _handers[eventDataType] = handlerTypes;
                    }
                    else
                    {
                        var handlerTypes = new List<Type>();
                        handlerTypes.Add(type);
                        _handers[eventDataType] = handlerTypes;
                    }
                }
            }
        }

        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            List<Type> handlers = new List<Type>();
            _handers.TryGetValue(typeof(EventData), out handlers);

            if (handlers.Count > 0)
            {
                foreach (var handler in handlers)
                {
                    MethodInfo methodInfo = handler.GetMethod("HandleEvent");
                    if (methodInfo!=null)
                    {
                        object obj = Activator.CreateInstance(handler);
                        methodInfo.Invoke(obj, new object[] { eventData });
                    }
                }
            }
        }
    }
}
