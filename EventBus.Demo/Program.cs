using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        private static void Test()
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
}
