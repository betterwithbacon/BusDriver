
using System;

namespace BusDriver.Core.Events.Time
{
	public class TimeEvent : IEvent
	{
		public DateTime Time { get;  set; }

		public IEventContext Context { get;  private set; }

		public TimeEvent(IEventContext context, DateTime time)
		{
			Context = context;
			Time = time;			
		}
	}
}