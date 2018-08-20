using BusDriver.Core.Logging;
using System.Collections.Generic;

namespace BusDriver.Core.Events
{
    public interface IEventProducer : ILogSource
    {
		void Init(IEventContext context);

		void Start();
	}
}
