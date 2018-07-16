namespace BusDriver.Core.Events
{
	public interface IEventContext
	{
		void RegisterProducer(IEventProducer eventSource);

		void HandleEvent(IEvent ev);

		void Initialize();

		void RegisterConsumer<TEvent>(IEventConsumer eventConsumer)
			where TEvent : IEvent;			
	}
}