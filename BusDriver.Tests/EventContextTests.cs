using BusDriver.Core.Events;
using BusDriver.Core.Events.Logging;
using BusDriver.Core.Events.Time;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using FluentAssertions;
using BusDriver.Core.Scheduling;
using BusDriver.Core.Logging;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using BusDriver.Core.Queueing;

namespace BusDriver.Tests
{
    public class EventContextTests
    {
		private readonly ITestOutputHelper output;
		public EventContextTests(ITestOutputHelper output)
		{
			this.output = output;
		}
	
		private EventContext GivenAContext()
		{
			var context = new EventContext();
			context.AddLogAction(
				(m) => output.WriteLine(m.ToString())
			);
			return context;
		}

		[Fact]
		[Trait("Tag", "Processing")]
		[Trait("Category", "Unit")]
		public void Do_ShouldPerformValidOperations()
		{
			var reached = false;

			GivenAContext()
				.Do(new Action<IEventContext>[] { (c) => { reached = true; } });

			reached.Should().BeTrue();
		}
	}
}
