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
            //MapEventToHandler();
        }

        static EventBus()
        {
            Default = new EventBus();

        }

        /// <summary>
        ///通过反射，将事件源与事件处理绑定
        /// </summary>
        private void MapEventToHandler()
        {
            //EventBusBootstrapper.Startup();
            //Assembly assembly = Assembly.GetEntryAssembly();
            //if (assembly == null)
            //{
            //    return;
            //}
            //foreach (var type in assembly.GetTypes())
            //{
            //    if (typeof(IEventHandler).IsAssignableFrom(type))//判断当前类型是否实现了IEventHandler接口
            //    {
            //        Type handlerInterface = type.GetInterface("IEventHandler`1");//获取该类实现的泛型接口
            //        if (handlerInterface != null)
            //        {
            //            Type eventDataType = handlerInterface.GetGenericArguments()[0]; // 获取泛型接口指定的参数类型

            //            if (_eventAndHandlerMapping.ContainsKey(eventDataType))
            //            {
            //                List<IEventHandler> handlerTypes = _eventAndHandlerMapping[eventDataType];
            //                handlerTypes.Add(Activator.CreateInstance(type) as IEventHandler);
            //                _eventAndHandlerMapping[eventDataType] = handlerTypes;
            //            }
            //            else
            //            {
            //                var handlerTypes = new List<IEventHandler> { Activator.CreateInstance(type) as IEventHandler };
            //                _eventAndHandlerMapping[eventDataType] = handlerTypes;
            //            }
            //        }
            //    }
            //}
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
        /// <param name="eventHandler"></param>
        public void UnRegister<TEventData>(Type eventHandler)
        {
            List<Type> handlerTypes = _eventAndHandlerMapping[typeof(TEventData)];

            _eventAndHandlerMapping.GetOrAdd(typeof(TEventData), (type) => new List<Type>())
                .RemoveAll(t => t == eventHandler);
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
                    //if (handler == typeof(ActionEventHandler<TEventData>))
                    //{
                    //    var actionEventHandler = Activator.CreateInstance<ActionEventHandler<TEventData>>();
                    //    actionEventHandler.Action(eventData);
                    //}
                    //var handlerType = typeof(IEventHandler<>).MakeGenericType(typeof(TEventData));
                    var handlerType = handler.GetInterface("IEventHandler`1");
                    var eventHandler = IocContainer.Resolve(handlerType) as IEventHandler<TEventData>;
                    eventHandler?.HandleEvent(eventData);
                }
            }
        }
    }
}
