using EventBus.EventStore;
using Shouldly;
using Xunit;

namespace EventBus.Test
{
    public class InMemory_EventStore_Test
    {
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.IsEmpty.ShouldBe(true);
        }

        [Fact]
        public void After_Register_Should_Contain_The_Event()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.AddRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.HasRegisterForEvent(typeof(TestEventData)).ShouldBe(true);
        }

        [Fact]

        public void After_UnRegister_Event_Should_No_Longger_Exist()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.AddRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.RemoveRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.HasRegisterForEvent(typeof(TestEventData)).ShouldBe(false);
        }

        [Fact]
        public void After_Clear_Should_Be_Empty()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.AddRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.IsEmpty.ShouldBe(false);

            eventStore.Clear();

            eventStore.IsEmpty.ShouldBe(true);
        }
    }
}