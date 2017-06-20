using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //注册当前程序集中实现的所有IEventHandler<T>
            EventBus.Default.RegisterAllEventHandlerFromAssembly(Assembly.GetExecutingAssembly());
            EventBusTest();
            Console.WriteLine("***********************");
            DelegateTest();
        }

        private static void EventBusTest()
        {
            Console.WriteLine("事件总线来钓鱼：");
            Console.WriteLine("=======================");
            //1、初始化鱼竿
            var fishingRod = new FishingRod();

            //2、声明垂钓者
            var jeff = new FishingMan("圣杰");

            //3.分配鱼竿
            jeff.FishingRod = fishingRod;            

            //4、循环钓鱼
            while (jeff.FishCount < 5)
            {
                jeff.Fishing();
                Console.WriteLine("-------------------");
                //睡眠2s
                Thread.Sleep(2000);
            }
        }

        private static void DelegateTest()
        {
            Console.WriteLine("委托实现的发布订阅模式来钓鱼：");
            Console.WriteLine("=======================");
            //1、初始化鱼竿
            var fishingRod = new FishingRodWithDelegate();

            //2、声明垂钓者
            var jeff = new FishingMan("圣杰");

            //3.分配鱼竿
            jeff.FishingRod = fishingRod;

            //4、注册观察者（已通过反射动态注册）
            //fishingRod.FishingEvent += new FishingEventHandler().HandleEvent;

            //5、循环钓鱼
            while (jeff.FishCount < 5)
            {
                jeff.Fishing();
                Console.WriteLine("-------------------");
                //睡眠2s
                Thread.Sleep(2000);
            }
        }


    }
}
