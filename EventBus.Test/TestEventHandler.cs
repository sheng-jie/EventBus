using EventBus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Test
{
    public class TestEventHandler : IEventHandler<TestEventData>
    {
        public static int TestValue { get; set; }
        public void HandleEvent(TestEventData eventData)
        {
            TestValue = eventData.TestVaule;
        }
    }
}
