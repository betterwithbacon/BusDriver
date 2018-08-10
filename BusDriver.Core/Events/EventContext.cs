using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusDriver.Core.Events.Time;
using BusDriver.Core.Logging;
using BusDriver.Core.Scheduling;

namespace BusDriver.Core.Events
{
	public class EventContext : IEventContext
	{
		readonly ConcurrentBag<IEventProducer> Producers = new ConcurrentBag<IEventProducer>();
		readonly ConcurrentDictionary<Type, IList<IEventConsumer>>  Consumers = new ConcurrentDictionary<Type, IList<IEventConsumer>>();
		readonly ConcurrentBag<IEvent> AllEvents = new ConcurrentBag<IEvent>();
		public readonly ConcurrentBag<LogMessage> SessionLogMessages = new ConcurrentBag<LogMessage>();
		readonly List<Action<LogMessage>> SessionLogActions = new List<Action<LogMessage>>();
		TimeEventProducer GlobalClock { get; set; } // raise an event every minute, like a clock (a not very good clock)

		public EventContext(double defaultScheduleTimeIntervalInMilliseconds = 60 * 1000)
		{
			GlobalClock = new TimeEventProducer(defaultScheduleTimeIntervalInMilliseconds);
		}

		public void AddLogAction(Action<LogMessage> messageAction)
		{
			SessionLogActions.Add(messageAction);
		}

		public void RegisterProducer(IEventProducer eventProducer)
		{
			// add it
			Producers.Add(eventProducer);
			
			// and register this as the context with the producer
			eventProducer.Init(this, $"{eventProducer.GetType().Name}-{GetNewPseudoRandomString()}");

			Log(LogType.ProducerRegistered, source: eventProducer as ILogSource);
		}

		static string GetNewPseudoRandomString()
		{
			var rawId = Guid.NewGuid().ToString();
			return rawId.Substring(rawId.LastIndexOf('-') + 1);
		}

		public void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent
		{

			Consumers.GetOrAdd(typeof(TEvent), new List<IEventConsumer> { eventConsumer }); //?.Add(eventConsumer);

			eventConsumer.Init(this, $"{eventConsumer.GetType().Name}-{GetNewPseudoRandomString()}");

			Log(LogType.ConsumerRegistered, source: eventConsumer as ILogSource);
		}

		public void Initialize()
		{
			// the default action is to write it to the internal session messages store			
			AddLogAction((m) => SessionLogMessages.Add(m));

			// start the global clock, it'll emit events based on time.				
			RegisterProducer(GlobalClock);
		}

		public void RaiseEvent(IEvent ev, ILogSource logSource)
		{
			// log the event was raised within the context
			Log(LogType.EventSent, ev.ToString(), source: logSource);
			
			// then handle it (F&F)
			HandleEvent(ev); 
		}

		void HandleEvent(IEvent ev)
		{
			if (ev == null)
				return;

			_ = Task.Run(() => {
				   AllEvents.Add(ev);

				   if (Consumers.TryGetValue(ev.GetType(), out var consumers))
				   {
					   foreach (var consumer in consumers)
						   consumer.HandleEvent(ev);
				   }
			   });
		}

		public IEnumerable<IEvent> GetAllReceivedEvents(PointInTime pointInTime = null)
		{
			return AllEvents.Where(e => pointInTime == null || pointInTime <= e.Time).ToArray();
		}

		public void Log(LogType logType, string message = null, ILogSource source = null)
		{
			Log(new LogMessage() {
				LogType = logType,
				Time = DateTime.Now,
				Message = message,
				Source = source
			});
		}

		public void LogError(Exception exception, string message = null, ILogSource source = null)
		{
			Log(new LogMessage() {
				LogType = LogType.Error,
				Time = DateTime.Now,
				Exception = exception,
				Message = message ?? exception.Message,
				Source = source
			});
		}

		public void Log(LogMessage message)
		{
			foreach (var logAction in SessionLogActions)
			{
				_= Task.Run(() => logAction(message));
			}
		}

		public DateTime GetTimeNow()
		{
			return DateTime.Now;
		}

		public void AddSchedule(Schedule schedule, Action<DateTime> actionToPerform)
		{
			var consumer = new TimeEventConsumer();
			consumer.Schedules.Add(schedule);
			consumer.EventAction = (time) => actionToPerform(time);
			RegisterConsumer<TimeEvent>(consumer);
		}

		public void CreateSchedule<TAction>()
			where TAction : IScheduledAction
		{
			// a schedule embedded in a type? is that practical/valuable? (daily backup?)
		}
	}
}
