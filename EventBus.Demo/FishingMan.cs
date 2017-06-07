using EventBus.Handlers;
using System;

namespace EventBus.Demo
{
    /// <summary>
    ///     垂钓者（观察者）
    /// </summary>
    public class FishingMan : IEventHandler<IEventData>
    {
        public FishingMan(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public int FishCount { get; set; }

        /// <summary>
        /// 垂钓者自然要有鱼竿啊
        /// </summary>
        public FishingRod FishingRod { get; set; }

        public void Fishing()
        {
            this.FishingRod.ThrowHook(this);
        }
        public void HandleEvent(IEventData eventData)
        {
            if (eventData is FishingEventData)
            {
                var fishingEventData = eventData as FishingEventData;
                FishCount++;
                Console.WriteLine("{0}：钓到一条[{2}]，已经钓到{1}条鱼了！", Name, FishCount, fishingEventData.FishType);
            }
        }
    }
}
