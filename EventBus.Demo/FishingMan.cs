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

        public void Update(FishType type)
        {
            FishCount++;
            Console.WriteLine("{0}：钓到一条[{2}]，已经钓到{1}条鱼了！", Name, FishCount, type);
        }
    }
}
