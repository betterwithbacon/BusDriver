using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Events.Logging
{
	public class LogEventProducer : IEventProducer
	{
		public void Emit(IEvent ev)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IEvent> GetEvents(PointInTime pointInTime)
		{
			throw new NotImplementedException();
		}

		public void Init(IEventContext context)
		{
			throw new NotImplementedException();
		}
	}
}
