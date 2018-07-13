using BusDriver.Core.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Scheduling
{
	public class SchedulerService : ISchedulerService
	{
		ConcurrentBag<IEventProducer> Producers { get; set; }
		ConcurrentDictionary<Type, IList<IEventConsumer>> Consumers { get; set; } //<IEventSource> Sources { get; set; }

		public SchedulerService()
		{
			Producers = new ConcurrentBag<IEventProducer>();
		}

		public void RegisterProducer(IEventProducer eventSource)
		{
			throw new NotImplementedException();
		}

		public void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent
		{
			Consumers.GetOrAdd(typeof(TEvent), new List<IEventConsumer> { eventConsumer })?.Add(eventConsumer);
		}

		public void Start()
		{
			throw new NotImplementedException();
		}
	}
}
