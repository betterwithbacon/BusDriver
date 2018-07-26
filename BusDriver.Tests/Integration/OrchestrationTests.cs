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

namespace BusDriver.Tests.Integration
{
    public class OrchestrationTests
    {
		private readonly ITestOutputHelper output;
		public OrchestrationTests(ITestOutputHelper output)
		{
			this.output = output;
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
			var timeEventConsumer = new TimeEventConsumer()
				{
					EventAction = (caller,ev) => 
						ev.Context.RaiseEvent(
							new LogEvent(ev.Context)
							{
								Message = $"TimeEvent hit at: {ev.Time}", Time = ev.Context.GetTimeNow()
							}, null
						)
				};

			// create a schedule that will only fire the Action when the time matches the event time
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			// create a consumer that will see the evenets and write to the log
			var logWriteConsumer = new LogEventConsumer(); 
			
			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			context.RaiseEvent(new TimeEvent(context, time), null);
			context.AssertEventExists<TimeEvent>();
			context.AssertEventExists<LogEvent>();

			output.WriteLine(Environment.NewLine + "Warehouse Contents");
			logWriteConsumer.LogLines.ForEach(output.WriteLine);
			logWriteConsumer.LogLines.Where(l => l.Contains(time.ToString())).Count().Should().Be(1);
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
			var timeEventConsumer = new TimeEventConsumer()
			{
				EventAction = (caller, ev) =>
					ev.Context.RaiseEvent(
						new LogEvent(ev.Context)
						{
							Message = $"TimeEvent hit at: {ev.Time}",
							Time = ev.Context.GetTimeNow()
						}, null
					)
			};

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

			context.AssertEventExists<TimeEvent>(3);
			context.AssertEventExists<LogEvent>(3);

			output.WriteLine(Environment.NewLine + "Warehouse Contents:");
			logWriteConsumer.LogLines.ForEach(output.WriteLine);
			logWriteConsumer.LogLines.Count().Should().Be(4);
		}

		[Fact]
		[Trait("Category", "Performance")]
		public void PerformanceTest_HundredsOfTimingEventsFinishesInAFewSeconds()
		{
			// create the orchestrator
			var context = new EventContext();
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);

			var time = DateTime.Parse("01/01/2018 10:00AM");

			// create a consumer that when it receives a time event, that matches it's schedule, it will trigger a log write event
			var timeEventConsumer = new TimeEventConsumer()
			{
				EventAction = (caller, ev) =>
					ev.Context.RaiseEvent(
						new LogEvent(ev.Context)
						{
							Message = $"Log of: TimeEvent hit at: {ev.Time.Ticks}",
							Time = ev.Context.GetTimeNow()
						}, caller
					)
			};

			// create a schedule that will only fire the Action when the time matches the event time
			timeEventConsumer.Schedules.Add(new Schedule { Frequency = ScheduleFrequency.OncePerDay, TimeToRun = time });

			var totalEventsSent = 100;

			// create a consumer that will see the evenets and write to the log
			var logWriteConsumer = new LogEventConsumer();

			context.RegisterConsumer<TimeEvent>(timeEventConsumer);
			context.RegisterConsumer<LogEvent>(logWriteConsumer);

			// run and ensure the listeners are all responding
			context.Initialize();

			var result = Parallel.ForEach(Enumerable.Range(1, totalEventsSent),
				new ParallelOptions { MaxDegreeOfParallelism = 10 },
				(index) => {
					output.WriteLine($"Test Event Emitted: {index}");
					context.RaiseEvent(new TimeEvent(context, time.AddDays(-1 * index)), null);
				}
			);

			output.WriteLine($"result: {result.IsCompleted}, {result.LowestBreakIteration}");
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

			//logWriteConsumer.LogLines.ForEach(output.WriteLine);

			logWriteConsumer.LogLines.Count().Should().Be(totalEventsSent + 1);
		}


		[Fact]
		[Trait("Category", "Performance")]
		public async Task PerformanceTest_HundredsOfHeterogeneousEventsFinishesInAFewSeconds()
		{
			// create the orchestrator
			var context = new EventContext();
			//context.AddLogAction(
			//	(m) => output.WriteLine(m.ToString())
			//);

			var time = DateTime.Parse("01/01/2018 10:00AM");

			// create a consumer that when it receives a time event, that matches it's schedule, it will trigger a log write event
			var timeEventConsumer = new TimeEventConsumer()
			{
				EventAction = (caller, ev) =>
					ev.Context.RaiseEvent(
						new LogEvent(ev.Context)
						{
							Message = $"Log of: TimeEvent hit at: {ev.Time.Ticks}",
							Time = ev.Context.GetTimeNow()
						}, caller
					)
			};

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
				.Select(index => context.RaiseEvent(new TimeEvent(context, time.AddDays(-1 * index)), null)); 
			//,new ParallelOptions { MaxDegreeOfParallelism = 5 },
			//	async (index)  => await context.RaiseEvent(new TimeEvent(context, time.AddDays(-1 * index)), null)				
			//);

			await Task.WhenAll(tasks);

			stopwatch.Stop();

			var netRunTime = stopwatch.ElapsedMilliseconds - overheadTime;

			output.WriteLine($"Total Events: totalEventsSent, Total runtime: {stopwatch.ElapsedMilliseconds}, Runtime: {netRunTime}ms. Avg Event Time: {(netRunTime/totalEventsSent).ToString("G")} ms/event. Events/Second: {(totalEventsSent/netRunTime).ToString("0.0000")} events/ms");

			// it should be able to process a hundred events in under a second
			stopwatch.ElapsedMilliseconds.Should().BeLessThan(totalEventsSent * (long)2); // each event should never take longer trhan 1.5milliseconds to run
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
