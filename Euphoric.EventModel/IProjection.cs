namespace BlazorEventsTodo.EventStorage
{
    public interface IProjection
    {
        IProjection NextState(IDomainEvent<IDomainEventData> evnt);
    }
}
