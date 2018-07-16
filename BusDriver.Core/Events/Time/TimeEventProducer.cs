
using System;
using System.Collections.Generic;

namespace BusDriver.Core.Events.Time
{
    public class TimeEventProducer : IEventProducer
	{	
		IEventContext Context { get; set; }

		public void Emit(IEvent ev)
		{
			AssertIsReady();

			Context?.HandleEvent(ev);
		}

		public IEnumerable<IEvent> GetEvents(PointInTime pointInTime)
		{
			// it's not obvious when this will be caused, so it should juts return a new time event
			yield return new TimeEvent(Context, DateTime.UtcNow);
		}

		public void Init(IEventContext context)
		{
			Context = context;
		}

		void AssertIsReady()
		{
			if (Context == null)
				throw new InvalidOperationException("Context is not set.");
		}
	}
}
