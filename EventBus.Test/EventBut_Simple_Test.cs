using Xunit;
using Shouldly;
using System;
using System.Collections.Generic;

namespace EventBus.Test
{
    public class EventBut_Simple_Test : EventBusTestBase
    {
        [Fact]
        public void Should_Call_Handler_On_Event_Has_Registered()
        {
            TestEventBus.Register<TestEventData>(typeof(TestEventHandler));

            TestEventBus.Trigger<TestEventData>(new TestEventData(1));
            TestEventHandler.TestValue.ShouldBe(1);
        }

        [Fact]
        public void Should_Throw_Exception_Without_Registered()
        {
            Assert.Throws<KeyNotFoundException>(() => TestEventBus.Trigger<TestEventData>(new TestEventData(1)));
        }

        [Fact]
        public void Should_Not_Trigger_On_UnRegistered()
        {
            Should_Call_Handler_On_Event_Has_Registered();

            TestEventBus.UnRegister<TestEventData>(typeof(TestEventHandler));

            TestEventBus.Trigger<TestEventData>(new TestEventData(2));

            TestEventHandler.TestValue.ShouldBe(1);

        }

    }
}
