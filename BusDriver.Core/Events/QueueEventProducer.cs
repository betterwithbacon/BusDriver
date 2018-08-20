using BusDriver.Core.Queueing;
using System;
using System.Linq;
using System.Threading;

namespace BusDriver.Core.Events
{
	public class QueueEventProducer : IEventProducer
	{
		public string Identifier { get; private set; }
		IWorkQueue<IEvent> WorkQueue { get; set; }
		IEventContext Context { get; set; }
		int DelayInMilliseconds { get; }
		Timer timer;

		public QueueEventProducer(IWorkQueue<IEvent> workQueue, int delayInMilliseconds = 1 * 1000)
		{
			WorkQueue = workQueue ?? throw new ApplicationException("No work queue was provided");
			DelayInMilliseconds = delayInMilliseconds;
		}

		public void Init(IEventContext context)
		{
			Identifier = EventContext.GenerateSessionIdentifier(this);			
		}

		public void Start()
		{
			// kick off the timer
			// TODO: the creation of the handler should be somewhere else probably
			timer = new Timer(
				(context) => {
					var ev = WorkQueue.Dequeue(1).FirstOrDefault();
					Context.RaiseEvent(ev, this);
				}, null, 100, 1000
			);
		}
	}
}
