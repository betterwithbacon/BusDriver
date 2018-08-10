using BusDriver.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Scheduling
{
    public interface IScheduledAction
    {
		Schedule Schedule { get; }
		Action<EventContext> ActionToPerform { get; }
    }
}
