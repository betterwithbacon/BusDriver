using BusDriver.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Queueing
{
    public interface IWorkQueue<out T>
    {
		IEnumerable<T> Dequeue(int count);
		void Enqueue(IEvent ev);
    }
}
