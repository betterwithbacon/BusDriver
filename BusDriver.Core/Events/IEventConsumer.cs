using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Events
{
	public interface IEventConsumer
	{
		void HandleEvent(IEvent ev);
	}
}
