using System;

namespace ObservePattern
{
    /// <summary>
    /// 鱼的品类枚举
    /// </summary>
    public enum FishType
    {
        鲫鱼,
        鲤鱼,
        黑鱼,
        青鱼,
        草鱼,
        鲈鱼
    }
    /// <summary>
    ///     鱼竿
    /// </summary>
    public class FishingRod
    {
        public delegate void FishingHandler(FishType type); //声明委托
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
                    FishingEvent(type);
            }
        }
    }
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

    private static void DelegateObserverTest()
    {
        Console.WriteLine("委托实现的观察者模式：");
        Console.WriteLine("=======================");
        //1、初始化鱼竿
        var fishingRod = new FishingRod();

        //2、声明垂钓者
        var jeff = new FishingMan("圣杰");

        //3、注册观察者
        fishingRod.FishingEvent += jeff.Update;

        //4、循环钓鱼
        while (jeff.FishCount < 5)
        {
            fishingRod.Fishing();
            Console.WriteLine("-------------------");
            //睡眠5s
            Thread.Sleep(5000);
        }
    }
}
