using BusDriver.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusDriver.Core.Events
{
	public interface IEventContext
	{
		void Initialize();

		void RegisterProducer(IEventProducer eventSource);

		void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent;

		Task RaiseEvent(IEvent ev, ILogSource source = null);

		Task Log(LogType logType, string message = null, ILogSource source = null);

		Task LogError(Exception exception, string message = null, ILogSource source = null);

		DateTime GetTimeNow();		
	}
}