using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Test
{
    public abstract class EventBusTestBase 
    {
        protected IEventBus TestEventBus;

        public EventBusTestBase()
        {
            TestEventBus = new EventBus();
        }
    }
}
