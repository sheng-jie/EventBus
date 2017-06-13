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
            TestEventBus.Register<TestEventData>(new TestEventHandler());

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
            var eventHandler = new TestEventHandler();
            TestEventBus.Register<TestEventData>(new TestEventHandler());

            TestEventBus.Trigger<TestEventData>(new TestEventData(1));
            TestEventHandler.TestValue.ShouldBe(1);

            TestEventBus.UnRegister<TestEventData>(eventHandler.GetType());

            TestEventBus.Trigger<TestEventData>(new TestEventData(2));

            TestEventHandler.TestValue.ShouldBe(1);

        }

        [Fact]
        public void Shoud_Call_Action_Handler()
        {
            var count = 0;
            TestEventBus.Register<EventData>(
                actionEventData => { count++; }
                );
            TestEventBus.Trigger(new EventData());

            count.ShouldBe(1);
        }
    }
}