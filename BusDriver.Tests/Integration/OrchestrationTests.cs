using BusDriver.Core.Events;
using BusDriver.Core.Events.Logging;
using BusDriver.Core.Events.Time;
using BusDriver.Core.Scheduling;
using NSubstitute;
using System;
using Xunit;

namespace BusDriver.Tests.Integration
{
    public class OrchestrationTests
    {
        [Fact]
		[Trait("","")]
        public void TimeEventShouldTriggerLogWriteEventWhichShouldThenWriteToLog()
        {
			// create the orchestrator
			var scheduler = new SchedulerService();

			// load up producers			
			var timeEventProducer = new TimeEventProducer();
			var timeEventConsumer = new TimeEventConsumer();
			var logWriteConsumer = new LogEventConsumer();
			
			scheduler.RegisterProducer(timeEventProducer);
			scheduler.RegisterConsumer<TimeEvent>(timeEventConsumer);
			scheduler.RegisterConsumer<LogEvent>(logWriteConsumer);

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
