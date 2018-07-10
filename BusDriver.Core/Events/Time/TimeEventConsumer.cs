
using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Events.Time
{
	public class TimeEventConsumer : IEventConsumer
	{
		public readonly Type[] Consumes = new[] { typeof(TimeEvent) };

		public void HandleEvent(IEvent ev)
		{
			if (!this.CanConsume(ev))
				throw new InvalidEventException(ev.GetType(), this.Consumes);

			throw new NotImplementedException();
		}
	}

	public static class EventConsumerExtensions
	{
		public static bool CanConsume(this IEventConsumer consumer, IEvent ev)
		{
			return true;
		}
	}
}
