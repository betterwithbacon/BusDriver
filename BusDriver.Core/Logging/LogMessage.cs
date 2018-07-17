using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Logging
{
	public class LogMessage
	{
		public DateTime Time { get; set; }
		public string Message { get; set; }
		public ILogSource Source { get; set; }
		public LogType LogType { get; set; }
		public Exception Exception { get; set; }

		public override string ToString()
		{
			return $"[{Source?.LogDescriptor.ToString() ?? "event"}] {Time}: {Message}";
		}
	}
}
