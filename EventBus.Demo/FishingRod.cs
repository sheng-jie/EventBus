using System;

namespace EventBus.Demo
{
    /// <summary>
    ///     鱼竿
    /// </summary>
    public class FishingRod
    {
        public delegate void FishingHandler(IEventData eventData); //声明委托
        public event FishingHandler FishingEvent; //声明事件

        public void Fishing()
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
                    var eventData = new FishingEventData() { FishType = type };
                    FishingEvent(eventData);
                }
            }
        }
    }
}
