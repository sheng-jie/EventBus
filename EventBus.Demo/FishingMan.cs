using EventBus.Handlers;
using System;

namespace EventBus.Demo
{
    /// <summary>
    ///     垂钓者（观察者）
    /// </summary>
    public class FishingMan
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
    }
}
