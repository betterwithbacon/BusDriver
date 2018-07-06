using BusDriver.Core.Events.Contracts;

namespace BusDriver.Core.Scheduling.Contracts
{
	public interface ISchedulerService
	{
		void RegisterProducer(IEventSource eventSource);
	}
}