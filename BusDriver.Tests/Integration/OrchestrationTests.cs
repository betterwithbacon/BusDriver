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
using BusDriver.Core.Logging;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using BusDriver.Core.Queueing;

namespace BusDriver.Tests.Integration
{
    public class OrchestrationTests
    {	
		private readonly ITestOutputHelper output;
		public OrchestrationTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		private EventContext GivenAContext()
		{
			var context = new EventContext();
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);
			return context;
		}

		[Fact]
		[Trait("Category", "Unit")]
		public void QueueEventProducerShouldRetrieveEventAndPutIntoContext()
		{
			var memQueue = new MemoryEventQueue();
			var context = new EventContext(eventQueue: memQueue);
				context.AddLogAction(
					(m) => output.WriteLine(m.ToString())
			);

			context.Initialize();

			var testEvent = new TestEvent(context);

			// raise the event
			//context.RaiseEvent(testEvent, null);
			memQueue.Enqueue(testEvent);

			// give the system enough time to react to the event showing up
			Thread.Sleep(200);

			// look for non-time events
			context.AssertEventExists<TestEvent>();
		}

		[Fact]
		[Trait("Category", "Unit")]
		public void TimeEventShouldTriggerLogWriteEventWhichShouldThenWriteToLog()
        {
			// create the orchestrator
			var context = new EventContext();
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);
						
			var time = DateTime.Parse("01/01/2018 10:00AM");

			// create a consumer that when it receives a time event, that matches it's schedule, it will trigger a log write event
			var timeEventConsumer = new TimeEventConsumer();
			timeEventConsumer.EventAction = (triggerTime) => context.RaiseEvent(
				new LogEvent(context)
				{
					Message = $"Log of: TimeEvent hit at: {triggerTime.Ticks}",
					Time = context.GetTimeNow()
				}, timeEventConsumer
			);

			// create a schedule that will only fire the Action when the time matches the event time
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			// create a consumer that will see the evenets and write to the log
			var logWriteConsumer = new LogEventConsumer(); 
			
			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			context.RaiseEvent(new TimeEvent(context, time), null);
			Thread.Sleep(500); // just wait a bit for the events to be handled
			context.AssertEventExists<TimeEvent>();
			context.AssertEventExists<LogEvent>();

			output.WriteLine(Environment.NewLine + "Warehouse Contents");
			logWriteConsumer.LogLines.ForEach(output.WriteLine);
			logWriteConsumer.LogLines.Where(l => l.Contains(time.Ticks.ToString())).Count().Should().Be(1);
		}

		[Fact]
		[Trait("Category", "Unit")]
		public void TimeEventShouldTriggerOneAndOnlyOneSchedule()
		{
			// create the orchestrator
			var context = new EventContext();
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);

			var time = DateTime.Parse("01/01/2018 10:00AM");

			// create a consumer that when it receives a time event, that matches it's schedule, it will trigger a log write event
			var timeEventConsumer = new TimeEventConsumer();
			timeEventConsumer.EventAction = (triggerTime) => context.RaiseEvent(
				new LogEvent(context)
				{
					Message = $"Log of: TimeEvent hit at: {triggerTime.Ticks}",
					Time = context.GetTimeNow()
				}, timeEventConsumer
			);

			// create a schedule that will only fire the Action when the time matches the event time
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			// create a consumer that will see the evenets and write to the log
			var logWriteConsumer = new LogEventConsumer();

			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			context.RaiseEvent(new TimeEvent(context, time), null);
			context.RaiseEvent(new TimeEvent(context, time.AddDays(-10)), null);
			context.RaiseEvent(new TimeEvent(context, time.AddDays(10)), null);

			Thread.Sleep(500); // just wait a bit for the events to be handled

			context.AssertEventExists<TimeEvent>(3);
			context.AssertEventExists<LogEvent>(3);

			output.WriteLine(Environment.NewLine + "Warehouse Contents:");
			logWriteConsumer.LogLines.ForEach(output.WriteLine);
			logWriteConsumer.LogLines.Count().Should().Be(4);
		}

		[Fact]
		[Trait("Category", "Unit")]
		public void Context_AddSchedule_ShouldAddCauseEventToBeTriggeredFromSchedule()
		{
			bool reached = false;

			// create the orchestrator
			var context = new EventContext(defaultScheduleTimeIntervalInMilliseconds:25);
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);

			var timeSecond = DateTime.Now.Second;

			// create a schedule that will only fire the Action when the time matches the event time
			context.AddScheduledAction(new Schedule { Frequency = ScheduleFrequency.OnceEveryUnitsMinute, FrequencyUnit = timeSecond },
				(hitTime) => { output.WriteLine($"EVENT HIT: {hitTime}"); reached = true; } );

			// run and ensure the listeners are all responding
			context.Initialize();

			Thread.Sleep(50); // just wait a bit for the events to be handled

			// a few time e vents may be release
			context.AssertEventExists<TimeEvent>(atLeast: 1);
			reached.Should().BeTrue();
		}

		[Fact]
		[Trait("Category", "Performance")]
		public async Task PerformanceTest_HundredsOfTimingEventsFinishesInAFewSeconds()
		{
			// create the orchestrator
			var context = new EventContext();
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);

			var time = DateTime.Parse("01/01/2018 10:00AM");

			// create a consumer that when it receives a time event, that matches it's schedule, it will trigger a log write event
			var timeEventConsumer = new TimeEventConsumer();
			timeEventConsumer.EventAction = (triggerTime) => context.RaiseEvent(
				new LogEvent(context)
				{
					Message = $"Log of: TimeEvent hit at: {triggerTime.Ticks}",
					Time = context.GetTimeNow()
				}, timeEventConsumer
			);
			
			// create a schedule that will only fire the Action when the time matches the event time
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			var totalEventsSent = 100;

			// create a consumer that will see the evenets and write to the log
			var logWriteConsumer = new LogEventConsumer();

			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			var tasks = Enumerable.Range(1, totalEventsSent)
				.Select(index => Task.Run( () => context.RaiseEvent(new TimeEvent(context, time.AddDays(-1 * index)), null)));

			await Task.WhenAll(tasks);

			context.AssertEventExists<TimeEvent>(totalEventsSent);
			context.AssertEventExists<LogEvent>(totalEventsSent);

			output.WriteLine(Environment.NewLine + "Warehouse Contents:");

			Parallel.ForEach(logWriteConsumer.LogLines,
				new ParallelOptions { MaxDegreeOfParallelism = 10 },
				(line) =>
				{
					output.WriteLine(line);
				}
			);

			logWriteConsumer.LogLines.Count().Should().Be(totalEventsSent + 1);
		}

		[Fact]
		[Trait("Category", "Performance")]
		public async Task PerformanceTest_HundredsOfHeterogeneousEventsFinishesInAFewSeconds()
		{
			// create the orchestrator
			var context = new EventContext();
			
			var time = DateTime.Parse("01/01/2018 10:00AM");

			// create a consumer that when it receives a time event, that matches it's schedule, it will trigger a log write event
			var timeEventConsumer = new TimeEventConsumer();
			timeEventConsumer.EventAction = (triggerTime) => context.RaiseEvent(
				new LogEvent(context)
				{
					Message = $"Log of: TimeEvent hit at: {triggerTime.Ticks}",
					Time = context.GetTimeNow()
				}, timeEventConsumer
			);

			// create a schedule that will only fire the Action when the time matches the event time
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			var totalEventsSent = 10_000;

			// create a consumer that will see the evenets and write to the log
			var logWriteConsumer = new LogEventConsumer();

			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			var stopwatch = new Stopwatch();

			var overheadTime = 0m;
			stopwatch.Start();

			Parallel.ForEach(Enumerable.Range(1, totalEventsSent), new ParallelOptions { MaxDegreeOfParallelism = 10 },
				(index) => { } //context.RaiseEvent(new TimeEvent(context, time.AddDays(-1 * index)), null)				
			);

			stopwatch.Stop();

			overheadTime = stopwatch.ElapsedMilliseconds;

			// see how long it takes for the events to be emitted and then responded too
			stopwatch.Restart();

			var tasks = Enumerable.Range(1, totalEventsSent)
				.Select(index => Task.Run(() => context.RaiseEvent(new TimeEvent(context, time.AddDays(-1 * index)), null))); 

			await Task.WhenAll(tasks);

			stopwatch.Stop();

			var netRunTime = stopwatch.ElapsedMilliseconds - overheadTime;

			output.WriteLine($"Total Events: totalEventsSent, Total runtime: {stopwatch.ElapsedMilliseconds}, Runtime: {netRunTime}ms. Avg Event Time: {(netRunTime/totalEventsSent).ToString("G")} ms/event. Events/Second: {(totalEventsSent/netRunTime).ToString("0.0000")} events/ms");

			// it should be able to process a hundred events in under a second
			stopwatch.ElapsedMilliseconds.Should().BeLessThan(totalEventsSent * (long)2); // each event should never take longer trhan 1.5milliseconds to run
		}

		private class TestEvent : IEvent
		{
			public TestEvent(IEventContext context, DateTime? time = null)
			{
				Context = context;
				Time = time ?? DateTime.Now;
			}

			public IEventContext Context { get; }

			public DateTime Time { get; private set; }
		}
	}

	public static class SchedulerTests
	{
		public static void AssertEventExists<TEvent>(this EventContext context, int count = 1, Func<IEvent, bool> additionalFilter = null, int? atLeast = null)
			where TEvent : IEvent
		{
			var recCount = context.GetAllReceivedEvents().Where(e => e.GetType() == typeof(TEvent) && (additionalFilter?.Invoke(e) ?? true)).Count();

			if (atLeast.HasValue)
				recCount.Should().BeGreaterOrEqualTo(atLeast.Value);
			else
				recCount.Should().Be(count);
		}
	}
}
