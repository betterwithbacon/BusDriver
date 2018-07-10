using BusDriver.Core.Events;
using BusDriver.Core.Scheduling.Models;
using NSubstitute;
using System;
using Xunit;

namespace BusDriver.Tests
{
    public class OrchestrationTests
    {
        [Fact]
		[Trait("","")]
        public void BasicStartup()
        {
			// create the orchestrator
			var scheduler = new SchedulerService();

			// load up producers			
			var eventSource = Substitute.For<IEventSource>();
			var eventSource2 = Substitute.For<IEventSource>();
			scheduler.RegisterProducer(eventSource);
			scheduler.RegisterProducer(eventSource2);

			// run and ensure the listeners are all responding

		}


		//[TestMethod]
		//public void EventSourceTests_GetEvents()
		//{
		//	var source = new TimeEventSource();

		//	var now = DateTime.UtcNow;
		//	var timeEvent = source.GetEvents().OfType<TimeEvent>().Single();

		//	Assert.IsTrue(timeEvent.Time.TimeOfDay.Subtract(now.TimeOfDay).Minutes < 1, "The time diff should be less than a minute");
		//}
	}
}
