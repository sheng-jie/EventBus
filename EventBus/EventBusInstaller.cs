using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EventBus.Handlers;

namespace EventBus
{
    public class EventBusInstaller : IWindsorInstaller
    {
        private IWindsorContainer _container;
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Register(
            //    Component.For<IEventBus>().ImplementedBy<EventBus>().LifestyleSingleton()
            //);

            //_eventBus = container.Resolve<IEventBus>();
            container.Register(Classes.FromAssembly(Assembly.GetExecutingAssembly()).BasedOn<IEventHandler>().LifestyleSingleton());
            _container = container;
            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }


        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            /* This code checks if registering component implements any IEventHandler<TEventData> interface, if yes,
             * gets all event handler interfaces and registers type to Event Bus for each handling event.
             */
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
                    var handlerType = typeof(IEventHandler<>).MakeGenericType(handler.ComponentModel.Implementation);
                    var eventHandler = _container.Resolve(handlerType) as IEventHandler;
                    EventBus.Default.Register(genericArgs[0], eventHandler);
                }
            }
        }
    }
}
