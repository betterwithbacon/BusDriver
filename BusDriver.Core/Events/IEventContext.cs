using BusDriver.Core.Logging;
using System;
using System.Collections.Generic;

namespace BusDriver.Core.Events
{
	public interface IEventContext
	{
		void RegisterProducer(IEventProducer eventSource);

		void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent;

		void HandleEvent(IEvent ev);

		void Initialize();

		void Log(LogType logType, string message = null, ILogSource source = null);

		void LogError(Exception exception, string message = null, ILogSource source = null);
	}
}