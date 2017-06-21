using Xunit;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus.Handlers;

namespace EventBus.Test
{
    public class EventBut_Simple_Test : EventBusTestBase
    {
        /// <summary>
        /// 默认实现的IEventHandler通过程序集默认已经注册
        /// 可直接触发事件
        /// </summary>
        [Fact]
        public void Should_Auto_Call_Without_Registered()
        {
            TestEventBus.Trigger<TestEventData>(new TestEventData(1));
            TestEventHandler.TestValue.ShouldBe(1);
        }

        [Fact]
        public void Should_Call_Handler_On_Event_Has_Registered()
        {
            TestEventBus.Register<TestEventData>(new TestEventHandler());

            TestEventBus.Trigger<TestEventData>(new TestEventData(1));
            TestEventHandler.TestValue.ShouldBe(1);
        }

        /// <summary>
        /// 已经卸载的IEventHandler，无法触发
        /// </summary>

        [Fact]
        public void Should_Not_Trigger_After_UnRegistered()
        {
            TestEventBus.UnRegister<TestEventData>(typeof(TestEventHandler));

            TestEventBus.Trigger<TestEventData>(new TestEventData(2));

            TestEventHandler.TestValue.ShouldBe(0);

        }

        [Fact]
        public void Should_Not_Trigger_After_UnRegisterAll()
        {
            TestEventHandler.TestValue = 0;
            var count = 0;
            TestEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );

            TestEventBus.UnRegisterAll<TestEventData>();

            TestEventBus.Trigger<TestEventData>(new TestEventData(1));

            TestEventHandler.TestValue.ShouldBe(0);

        }

        /// <summary>
        /// 注册的Action可以触发成功
        /// </summary>
        [Fact]
        public void Should_Call_Action_Handler()
        {
            var count = 0;
            TestEventBus.Register<EventData>(
                actionEventData => { count++; }
                );
            TestEventBus.Trigger(new EventData());

            count.ShouldBe(1);
        }

        /// <summary>
        /// 若一个事件源注册了多个EventHandler,则触发多次
        /// </summary>
        [Fact]
        public void Should_Call_All_Handlers()
        {
            var count = 0;
            TestEventBus.Register<TestEventData>(
                actionEventData => { count++; }
                );

            TestEventBus.Trigger<TestEventData>(new TestEventData(999));

            TestEventHandler.TestValue.ShouldBe(999);
            count.ShouldBe(1);
        }


        [Fact]
        public async Task Should_Call_Handler_Async()
        {
            await TestEventBus.TriggerAsync(new TestEventData(123));
            TestEventHandler.TestValue.ShouldBe(123);
        }

        [Fact]
        public void Should_Call_Specified_Handler()
        {
            TestEventHandler.TestValue = 0;
            TestEventBus.Register<TestEventData>(new TestEventHandler());
            var count = 0;
            TestEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );

            TestEventBus.Trigger<TestEventData>(typeof(TestEventHandler), new TestEventData(999));

            TestEventHandler.TestValue.ShouldBe(999);
            count.ShouldBe(0);
        }

        [Fact]
        public async void Should_Call_Specified_Handler_Async()
        {
            TestEventHandler.TestValue = 0;
            TestEventBus.Register<TestEventData>(new TestEventHandler());

            var count = 0;
            TestEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );
            await TestEventBus.TriggerAsycn<TestEventData>
                (typeof(TestEventHandler), new TestEventData(999));

            TestEventHandler.TestValue.ShouldBe(999);
            count.ShouldBe(0);
        }


    }
}