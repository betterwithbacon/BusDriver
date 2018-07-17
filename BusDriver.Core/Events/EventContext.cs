using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BusDriver.Core.Logging;

namespace BusDriver.Core.Events
{
	public class EventContext : IEventContext
	{
		readonly ConcurrentBag<IEventProducer> Producers = new ConcurrentBag<IEventProducer>();
		readonly ConcurrentDictionary<Type, IList<IEventConsumer>>  Consumers = new ConcurrentDictionary<Type, IList<IEventConsumer>>();
		readonly ConcurrentBag<IEvent> AllEvents = new ConcurrentBag<IEvent>();
		public readonly ConcurrentBag<LogMessage> SessionLogMessages = new ConcurrentBag<LogMessage>();
		readonly List<Action<LogMessage>> SessionLogActions = new List<Action<LogMessage>>();

		public EventContext()
		{
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

			Log(LogType.ProducerRegistered, $"{eventProducer} registered with Context");			
		}

		static string GetNewPseudoRandomString()
		{
			var rawId = Guid.NewGuid().ToString();
			return rawId.Substring(rawId.LastIndexOf('-') + 1);
		}

		public void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent
		{
			Consumers.GetOrAdd(typeof(TEvent), new List<IEventConsumer> { eventConsumer })?.Add(eventConsumer);
		}

		public void Initialize()
		{
			// the default action is to write it to the internal session messages store			
			AddLogAction((m) => SessionLogMessages.Add(m));

			// TODO: start the scheduler to poll for messages
		}

		public void HandleEvent(IEvent ev)
		{
			if (ev == null)
				return;

			AllEvents.Add(ev);

			if (Consumers.TryGetValue(ev.GetType(), out var consumers))
			{
				foreach (var consumer in consumers)
					consumer.HandleEvent(ev);
			}
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
				logAction(message);
		}
	}
}
