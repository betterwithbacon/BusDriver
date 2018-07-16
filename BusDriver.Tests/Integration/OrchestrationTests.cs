using BusDriver.Core.Events;
using BusDriver.Core.Events.Logging;
using BusDriver.Core.Events.Time;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using FluentAssertions;
using BusDriver.Core.Scheduling;

namespace BusDriver.Tests.Integration
{
    public class OrchestrationTests
    {
        [Fact]
		[Trait("","")]
        public void TimeEventShouldTriggerLogWriteEventWhichShouldThenWriteToLog()
        {
			// create the orchestrator
			var context = new EventContext();
			var time = DateTime.Parse("01/01/2018 10:00AM");

			// load up producers			
			var timeEventProducer = new TimeEventProducer();
			var timeEventConsumer = new TimeEventConsumer()
			{
				EventAction = (caller,ev) => ev.Context.HandleEvent(new LogEvent(ev.Context) { Message = $"TimeEvent hit at: {ev.Time}" })
			};
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			var logWriteConsumer = new LogEventConsumer(); 
			
			context.RegisterProducer(timeEventProducer);
			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			timeEventProducer.Emit(new TimeEvent(context, time));
			context.AssertEventExists<TimeEvent>();
			context.AssertEventExists<LogEvent>();
		}
	}

	public static class SchedulerTests
	{
		public static void AssertEventExists<TEvent>(this EventContext context, int count = 1, Func<IEvent, bool> additionalFilter = null)
			where TEvent : IEvent
		{
			context.GetAllReceivedEvents().Where(e => e.GetType() == typeof(TEvent) && (additionalFilter?.Invoke(e) ?? true)).Count().Should().Be(count);
		}
	}
}
