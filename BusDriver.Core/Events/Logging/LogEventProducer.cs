using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Events.Logging
{
	public class LogEventProducer : IEventProducer
	{
		public IEnumerable<IEvent> GetEvents()
		{
			throw new NotImplementedException();
		}
	}
}
