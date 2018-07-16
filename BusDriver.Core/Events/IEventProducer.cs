using System.Collections.Generic;

namespace BusDriver.Core.Events
{
    public interface IEventProducer
    {
		IEnumerable<IEvent> GetEvents(PointInTime pointInTime);

		void Emit(IEvent ev);

		void Init(IEventContext context);
	}
}
