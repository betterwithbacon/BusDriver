using System;

namespace BusDriver.Core.Events.Logging
{
	public class LogEvent : IEvent
	{
		public string Message { get; set; }

		public DateTime Time { get; private set; }

		public IEventContext Context { get; private set; }

		public LogEvent(IEventContext context)
		{
			Context = context;
		}
	}
}