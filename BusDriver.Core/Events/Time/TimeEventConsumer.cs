
using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Events.Time
{
	public class TimeEventConsumer : IEventConsumer
	{
		public IList<Type> Consumes =>  new [] { typeof(TimeEvent) };

		public void HandleEvent(IEvent ev)
		{
			throw new NotImplementedException();
		}
	}
}
