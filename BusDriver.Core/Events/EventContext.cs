using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BusDriver.Core.Events
{
	public class EventContext : IEventContext
	{
		ConcurrentBag<IEventProducer> Producers { get; set; }
		ConcurrentDictionary<Type, IList<IEventConsumer>> Consumers { get; set; }
		ConcurrentBag<IEvent> AllEvents { get; set; }

		public EventContext()
		{
			Consumers = new ConcurrentDictionary<Type, IList<IEventConsumer>>();
			Producers = new ConcurrentBag<IEventProducer>();
			AllEvents = new ConcurrentBag<IEvent>();
		}

		public void RegisterProducer(IEventProducer eventProducer)
		{
			// add it
			Producers.Add(eventProducer);

			// and register this as the context with the producer
			eventProducer.Init(this);			
		}

		public void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent
		{
			Consumers.GetOrAdd(typeof(TEvent), new List<IEventConsumer> { eventConsumer })?.Add(eventConsumer);
		}

		public void Initialize()
		{
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
	}
}
