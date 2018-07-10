using BusDriver.Core.Events;

using BusDriver.Core.Scheduling.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Scheduling.Models
{
	public class SchedulerService : ISchedulerService
	{
		ConcurrentBag<IEventSource> Sources { get; set; }
		ConcurrentDictionary<Type, IList<IEventConsumer>> Consumers { get; set; } //<IEventSource> Sources { get; set; }

		public SchedulerService()
		{
			Sources = new ConcurrentBag<IEventSource>();
		}

		public void RegisterProducer(IEventSource eventSource)
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
