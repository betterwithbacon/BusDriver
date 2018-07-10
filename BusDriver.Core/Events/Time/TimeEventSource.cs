
using System;
using System.Collections.Generic;

namespace BusDriver.Core.Events.Time
{
    public class TimeEventSource : IEventSource
    {
		public IEnumerable<IEvent> GetEvents()
		{
			// it's not obvious when this will be caused, so it should juts return a new time event
			yield return new TimeEvent(DateTime.UtcNow);
		}
    }
}
