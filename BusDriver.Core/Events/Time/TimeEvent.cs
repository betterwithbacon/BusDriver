
using System;

namespace BusDriver.Core.Events.Time
{
	public struct TimeEvent : IEvent
	{
		public DateTime Time { get;  private set; }

		public TimeEvent(DateTime time)
		{
			Time = time;
		}
	}
}