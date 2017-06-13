using EventBus.Handlers;
using System;
using System.Reflection;

namespace EventBus.Demo
{
    /// <summary>
    ///     鱼竿
    /// </summary>
    public class FishingRod
    {
        //public delegate void FishingHandler(FishingEventData eventData); //声明委托
        //public event FishingHandler FishingEvent; //声明事件

        public FishingRod()
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();

            //foreach (var type in assembly.GetTypes())
            //{
            //    if (typeof(IEventHandler).IsAssignableFrom(type))//判断当前类型是否实现了IEventHandler接口
            //    {
            //        Type handlerInterface = type.GetInterface("IEventHandler`1");//获取该类实现的泛型接口
            //        Type eventDataType = handlerInterface.GetGenericArguments()[0]; // 获取泛型接口指定的参数类型

            //        //如果参数类型是FishingEventData，则说明事件源匹配
            //        if (eventDataType == typeof(FishingEventData))
            //        {
            //            //创建实例
            //            var handler = Activator.CreateInstance(type) as IEventHandler<FishingEventData>;
            //            //注册事件
            //            if (handler != null) FishingEvent += handler.HandleEvent;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 下钩
        /// </summary>
        public void ThrowHook(FishingMan man)
        {
            Console.WriteLine("开始下钩！");

            //用随机数模拟鱼咬钩，若随机数为偶数，则为鱼咬钩
            if (new Random().Next() % 2 == 0)
            {
                var a = new Random(10).Next();
                var type = (FishType)new Random().Next(0, 5);
                Console.WriteLine("铃铛：叮叮叮，鱼儿咬钩了");
                //if (FishingEvent != null)
                //{
                var eventData = new FishingEventData() { FishType = type, FishingMan = man };
                //FishingEvent(eventData);//不再需要通过事件委托触发
                EventBus.Default.Trigger<FishingEventData>(eventData);//直接通过事件总线触发即可

                //EventBus.Default.Register<EventData>(
                //    actionEventData => { Console.WriteLine(actionEventData.GetType()); }
                //    );

                //EventBus.Default.Trigger(new EventData());
                //}
            }
        }
    }
}
