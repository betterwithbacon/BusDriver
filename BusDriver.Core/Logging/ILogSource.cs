using System;
using System.Collections.Generic;
using System.Text;

namespace BusDriver.Core.Logging
{
    public interface ILogSource
    {
		string Identifier { get; }
	}
}
