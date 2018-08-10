using BusDriver.Core.Logging;
using System;

namespace BusDriver.Core.Events
{
	public abstract class BaseEventProducer : IEventProducer
	{
		public string Identifier { get; private set; }

		public string LogDescriptor => Identifier;

		protected IEventContext Context { get; private set; }

		public void Init(IEventContext context, string identifier)
		{
			Context = context;
			Identifier = identifier;
			Context.Log(LogType.ProducerStartup, source: this);

			Start();
		}

		public abstract void Start();

		protected virtual void AssertIsReady()
		{
			if (Context == null)
				throw new InvalidOperationException("Context is not set.");
		}

		public override string ToString()
		{
			return Identifier;
		}
	}
}
