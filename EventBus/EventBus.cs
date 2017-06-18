using EventBus.Handlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace EventBus
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public class EventBus : IEventBus
    {
        public IWindsorContainer IocContainer { get; private set; }
        public static EventBus Default { get; private set; }

        /// <summary>
        /// 定义线程安全集合
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<Type>> _eventAndHandlerMapping;

        public EventBus()
        {
            IocContainer = new WindsorContainer();
            _eventAndHandlerMapping = new ConcurrentDictionary<Type, List<Type>>();
        }

        static EventBus()
        {
            Default = new EventBus();

        }

        /// <summary>
        /// 手动绑定事件源与事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventHandler"></param>
        public void Register<TEventData>(IEventHandler eventHandler)
        {
            Register(typeof(TEventData), eventHandler.GetType());
        }


        public void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            var actionHandler = new ActionEventHandler<TEventData>(action);
            IocContainer.Register(Component.For<IEventHandler<TEventData>>().UsingFactoryMethod(() => actionHandler)
                .LifestyleSingleton());
            Register<TEventData>(actionHandler);
        }

        public void Register(Type eventType, Type handlerType)
        {
            if (_eventAndHandlerMapping.Keys.Contains(eventType))
            {
                List<Type> handlerTypes = _eventAndHandlerMapping[eventType];
                if (!handlerTypes.Contains(handlerType))
                {
                    handlerTypes.Add(handlerType);
                    _eventAndHandlerMapping[eventType] = handlerTypes;
                }
            }
            else
            {
                _eventAndHandlerMapping.GetOrAdd(eventType, (type) => new List<Type>()).Add(handlerType);
            }
        }

        /// <summary>
        /// 提供入口支持注册其它程序集中实现的IEventHandler
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterAllEventHandlerFromAssembly(Assembly assembly)
        {
            IocContainer.Register(Classes.FromAssembly(assembly)
                .BasedOn(typeof(IEventHandler<>))
                .WithService.AllInterfaces()
                .LifestyleSingleton());

            var handlers = IocContainer.Kernel.GetHandlers(typeof(IEventHandler));
            foreach (var handler in handlers)
            {
                if (!typeof(IEventHandler).IsAssignableFrom(handler.ComponentModel.Implementation))
                {
                    return;
                }

                var interfaces = handler.ComponentModel.Implementation.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).IsAssignableFrom(@interface))
                    {
                        continue;
                    }

                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        Register(genericArgs[0], handler.ComponentModel.Implementation);
                    }
                }
            }
        }

        /// <summary>
        /// 手动解除事件源与事件处理的绑定
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="handlerType"></param>
        public void UnRegister<TEventData>(Type handlerType)
        {
            List<Type> handlerTypes = _eventAndHandlerMapping[typeof(TEventData)];

            _eventAndHandlerMapping.GetOrAdd(typeof(TEventData), (type) => new List<Type>())
                .RemoveAll(t => t == handlerType);
        }

        /// <summary>
        /// 根据事件源触发绑定的事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            List<Type> handlers = _eventAndHandlerMapping[typeof(TEventData)];

            if (handlers != null && handlers.Count > 0)
            {
                foreach (var handler in handlers)
                {
                    var handlerType = handler.GetInterface("IEventHandler`1");
                    var eventHandler = IocContainer.Resolve(handlerType) as IEventHandler<TEventData>;
                    eventHandler?.HandleEvent(eventData);
                }
            }
        }
    }
}
