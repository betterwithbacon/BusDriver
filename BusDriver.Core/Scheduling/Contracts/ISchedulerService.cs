

using BusDriver.Core.Events;

namespace BusDriver.Core.Scheduling.Contracts
{
	public interface ISchedulerService
	{
		void RegisterProducer(IEventSource eventSource);
	}
}