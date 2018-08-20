using BusDriver.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Queueing
{
	// a simple in-memory queue for events.
	public class MemoryEventQueue : IWorkQueue<IEvent>
	{
		private Queue<IEvent> Queue { get; }

		public MemoryEventQueue(Queue<IEvent> eventQueue = null)
		{
			Queue = eventQueue ?? new Queue<IEvent>();
		}
		
		public IEnumerable<IEvent> Dequeue(int count)
		{
			for(int i = 0; i < count; i++)
				yield return Queue.Dequeue();
		}

		public void Enqueue(IEvent ev)
		{
			Queue.Enqueue(ev);
		}
	}
}
