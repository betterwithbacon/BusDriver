using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Logging
{
    public enum LogType
    {
		EventSent,
		EventReceived,

		ProducerRegistered,
		ProducerStartup,
		ProducerShutdown,

		ConsumerRegistered,
		ConsumerStartup,
		ConsumerShutdown,

		ContextStartup,
		ContextShutdown,
		ContextJoin,

		Error,
		Info
	}
}
