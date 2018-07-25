
using BusDriver.Core.Logging;
using System;
using System.Collections.Generic;

namespace BusDriver.Core.Events.Time
{
    public class TimeEventProducer : IEventProducer
	{
		public string Identifier { get; private set; }

		public string LogDescriptor => Identifier;

		IEventContext Context { get; set; }

		public TimeEventProducer()
		{
		}

		//public void Emit(IEvent ev)
		//{
		//	AssertIsReady();

		//	Context?.RaiseEvent(ev);
		//}

		public IEnumerable<IEvent> GetEvents(PointInTime pointInTime)
		{
			// it's not obvious when this will be caused, so it should juts return a new time event
			yield return new TimeEvent(Context, DateTime.UtcNow);
		}

		public void Init(IEventContext context, string identifier)
		{
			Context = context;			
			Identifier = identifier;
			Context.Log(LogType.ProducerStartup, source: this);			
		}

		void AssertIsReady()
		{
			if (Context == null)
				throw new InvalidOperationException("Context is not set.");
		}

		public override string ToString()
		{
			return Identifier;
		}
	}
}
