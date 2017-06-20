using EventBus.Handlers;
using System;
using System.Reflection;

namespace EventBus.Demo
{
    /// <summary>
    ///     鱼竿
    /// </summary>
    public class FishingRod : IFishingRod
    {
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

                var eventData = new FishingEventData() { FishType = type, FishingMan = man };

                EventBus.Default.Trigger<FishingEventData>(eventData);//直接通过事件总线触发

            }
        }
    }
}
