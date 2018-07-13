using System;
using System.Collections.Generic;
using System.Text;
using WarehouseCore;

namespace BusDriver.Core.Events.Logging
{
    public class LogEventConsumer : IEventConsumer
    {
		private readonly Warehouse warehouse = new Warehouse();
		private string LOG_NAME = "";

		public IList<Type> Consumes => new[] { typeof(LogEvent) };

		public void HandleEvent(IEvent ev)
		{
			this.ThrowIfInvalidEvent(ev);


		}
    }
}
