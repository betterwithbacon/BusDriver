using BusDriver.Core.Logging;
using System.Collections.Generic;

namespace BusDriver.Core.Events
{
    public interface IEventProducer : ILogSource
    {
		new string Identifier { get; }

		IEnumerable<IEvent> GetEvents(PointInTime pointInTime);

		void Init(IEventContext context, string identifier);
	}
}
