using System;
using System.Reflection;

namespace EventBus.Demo
{
    /// <summary>
    ///     鱼竿
    /// </summary>
    public class FishingRod
    {
        public delegate void FishingHandler(FishingEventData eventData); //声明委托
        public event FishingHandler FishingEvent; //声明事件

        public FishingRod()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {
                Type eventHandlerType = type.GetInterface("IEventHandler`1");//获取实现了IEventHandler的类型
                if (eventHandlerType != null) //若是事件处理者
                {
                    Type eventType = eventHandlerType.GetGenericArguments()[0]; // 获取IEventHandler的参数类型
                    //未完待续
                }
            }
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
                if (FishingEvent != null)
                {
                    var eventData = new FishingEventData() { FishType = type, FishingMan = man };
                    FishingEvent(eventData);
                }
            }
        }
    }
}
