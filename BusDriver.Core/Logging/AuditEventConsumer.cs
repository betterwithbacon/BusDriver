using BusDriver.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Logging
{
	public class AuditEventConsumer : IEventConsumer
	{
		public IList<Type> Consumes => new[] { typeof(IEvent) };

		public string Identifier { get; private set; }
				
		public Action<IEventConsumer, IEvent> EventAction { get; set; }

		public void HandleEvent(IEvent ev)
		{
			throw new NotImplementedException();
		}

		public void Init(IEventContext context, string identifier)
		{
			Context = context;
			Identifier = identifier;
			Context.Log(LogType.ConsumerStartup, source: this);
		}
	}
}
