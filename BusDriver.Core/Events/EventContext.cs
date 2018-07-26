using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

			// TODO: start the scheduler to poll for messages
		}

		public async Task RaiseEvent(IEvent ev, ILogSource logSource)
		{
			// log the event was raised within the context
			await Log(LogType.EventSent, ev.ToString(), source: logSource);
			
			// then handle it
			await HandleEvent(ev);
		}

		async Task HandleEvent(IEvent ev)
		{
			if (ev == null)
				return;

			await Task.Run(() => {
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

		public async Task Log(LogType logType, string message = null, ILogSource source = null)
		{
			await Log(new LogMessage() {
				LogType = logType,
				Time = DateTime.Now,
				Message = message,
				Source = source
			});
		}

		public async Task LogError(Exception exception, string message = null, ILogSource source = null)
		{
			await Log(new LogMessage() {
				LogType = LogType.Error,
				Time = DateTime.Now,
				Exception = exception,
				Message = message ?? exception.Message,
				Source = source
			});
		}

		public async Task Log(LogMessage message)
		{
			foreach (var logAction in SessionLogActions)
			{
				await Task.Run(() => logAction(message));
			}
		}

		public DateTime GetTimeNow()
		{
			return DateTime.Now;
		}
	}
}
