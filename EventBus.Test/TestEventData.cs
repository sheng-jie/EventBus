using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Test
{
    public class TestEventData : EventData
    {
        public int TestVaule { get; set; }

        public TestEventData(int data)
        {
            TestVaule = data;
        }

    }
}
