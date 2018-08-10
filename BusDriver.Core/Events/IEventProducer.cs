using BusDriver.Core.Logging;
using System.Collections.Generic;

namespace BusDriver.Core.Events
{
    public interface IEventProducer : ILogSource
    {
		new string Identifier { get; }

		void Init(IEventContext context, string identifier);
	}
}
