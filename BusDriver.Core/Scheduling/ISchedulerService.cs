

using BusDriver.Core.Events;

namespace BusDriver.Core.Scheduling
{
	public interface ISchedulerService
	{
		void RegisterProducer(IEventProducer eventSource);
	}
}