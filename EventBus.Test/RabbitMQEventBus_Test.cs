using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EventBus.RabbitMQ;
using Shouldly;
using Xunit;

namespace EventBus.Test
{
    public class RabbitMQEventBus_Test
    {
        protected IEventBus TestRabbitMQEventBus;
        public RabbitMQEventBus_Test()
        {
            TestRabbitMQEventBus = new RabbitMQEventBus();
            TestRabbitMQEventBus.RegisterAllEventHandlerFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 默认实现的IEventHandler通过程序集默认已经注册
        /// 可直接触发事件
        /// </summary>
        [Fact]
        public void Should_Auto_Call_Without_Registered()
        {
            TestRabbitMQEventBus.Trigger<TestEventData>(new TestEventData(1));
            Thread.Sleep(1000);
            TestEventHandler.TestValue.ShouldBe(1);
        }

        [Fact]
        public void Should_Call_Handler_On_Event_Has_Registered()
        {
            TestRabbitMQEventBus.UnRegisterAll<TestEventData>();
            TestRabbitMQEventBus.Register<TestEventData>(new TestEventHandler());

            TestRabbitMQEventBus.Trigger<TestEventData>(new TestEventData(2));
            TestEventHandler.TestValue.ShouldBe(2);
        }

        /// <summary>
        /// 已经卸载的IEventHandler，无法触发
        /// </summary>

        [Fact]
        public void Should_Not_Trigger_After_UnRegistered()
        {
            TestRabbitMQEventBus.UnRegister<TestEventData>(typeof(TestEventHandler));

            TestRabbitMQEventBus.Trigger<TestEventData>(new TestEventData(2));

            TestEventHandler.TestValue.ShouldBe(0);

        }

        [Fact]
        public void Should_Not_Trigger_After_UnRegisterAll()
        {
            TestEventHandler.TestValue = 0;
            var count = 0;
            TestRabbitMQEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );

            TestRabbitMQEventBus.UnRegisterAll<TestEventData>();

            TestRabbitMQEventBus.Trigger<TestEventData>(new TestEventData(1));

            count.ShouldBe(0);
            TestEventHandler.TestValue.ShouldBe(0);

        }

        /// <summary>
        /// 注册的Action可以触发成功
        /// </summary>
        [Fact]
        public void Should_Call_Action_Handler()
        {
            var count = 0;
            TestRabbitMQEventBus.Register<EventData>(
                actionEventData => { count++; }
            );
            TestRabbitMQEventBus.Trigger(new EventData());
            Thread.Sleep(1000);
            count.ShouldBe(1);
        }

        /// <summary>
        /// 若一个事件源注册了多个EventHandler,则触发多次
        /// </summary>
        [Fact]
        public void Should_Call_All_Handlers()
        {
            var count = 0;
            TestRabbitMQEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );

            TestRabbitMQEventBus.Trigger<TestEventData>(new TestEventData(999));

            Thread.Sleep(1000);
            TestEventHandler.TestValue.ShouldBe(999);
            count.ShouldBe(1);
        }


        [Fact]
        public async Task Should_Call_Handler_Async()
        {
            await TestRabbitMQEventBus.TriggerAsync(new TestEventData(123));
            TestEventHandler.TestValue.ShouldBe(123);
        }

        [Fact]
        public void Should_Call_Specified_Handler()
        {
            TestEventHandler.TestValue = 0;
            TestRabbitMQEventBus.Register<TestEventData>(new TestEventHandler());
            var count = 0;
            TestRabbitMQEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );

            TestRabbitMQEventBus.Trigger<TestEventData>(typeof(TestEventHandler), new TestEventData(999));

            TestEventHandler.TestValue.ShouldBe(999);
            count.ShouldBe(0);
        }

        [Fact]
        public async void Should_Call_Specified_Handler_Async()
        {
            TestEventHandler.TestValue = 0;
            TestRabbitMQEventBus.Register<TestEventData>(new TestEventHandler());

            var count = 0;
            TestRabbitMQEventBus.Register<TestEventData>(
                actionEventData => { count++; }
            );
            await TestRabbitMQEventBus.TriggerAsycn<TestEventData>
                (typeof(TestEventHandler), new TestEventData(999));

            TestEventHandler.TestValue.ShouldBe(999);
            count.ShouldBe(0);
        }

    }
}