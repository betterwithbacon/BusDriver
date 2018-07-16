﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Events
{
	public interface IEvent
	{
		IEventContext Context { get; }
		DateTime Time { get; }
	}
}
