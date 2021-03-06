﻿using System;
using BusDriver.Core.Events.Time;

namespace BusDriver.Core.Events
{
	public class EventAction
	{
		public Action<IEvent> Action { get; set; }

		public void Run(IEventActionTrigger trigger, IEvent ev)
		{
			Action?.Invoke(ev);
		}
	}
}