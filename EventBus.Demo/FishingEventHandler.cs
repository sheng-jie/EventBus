using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Handlers;

namespace EventBus.Demo
{
    /// <summary>
    /// 钓鱼事件处理
    /// </summary>
    public class FishingEventHandler : IEventHandler<FishingEventData>
    {
        public void HandleEvent(FishingEventData eventData)
        {
            eventData.FishingMan.FishCount++;

            Console.WriteLine("{0}：钓到一条[{2}]，已经钓到{1}条鱼了！",
                eventData.FishingMan.Name, eventData.FishingMan.FishCount, eventData.FishType);

        }
    }

    public class FishingEventHandler2 : IEventHandler<FishingEventData>
    {
        public void HandleEvent(FishingEventData eventData)
        {

            Console.WriteLine("哈哈哈哈哈！");

        }
    }
}
