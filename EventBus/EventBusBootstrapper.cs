using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace EventBus
{
    public static class EventBusBootstrapper
    {
        public static void Startup()
        {
            var container = new WindsorContainer();
            container.Install(new EventBusInstaller());
        }
    }
}
