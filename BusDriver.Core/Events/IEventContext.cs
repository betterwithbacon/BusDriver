using BusDriver.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusDriver.Core.Events
{
	public interface IEventContext
	{
		string Id { get; }

		void Initialize();		

		void RegisterProducer(IEventProducer eventSource);

		void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent;

		void RaiseEvent(IEvent ev, ILogSource source = null);


		void Log(LogType logType, string message = null, ILogSource source = null);

		void LogError(Exception exception, string message = null, ILogSource source = null);

		DateTime GetTimeNow();		
	}
}