namespace BlazorEventsTodo.EventStorage
{
    public interface IProjectionContainerFactory
    {
        IProjectionState<TProjection> CreateProjectionState<TProjection>()
            where TProjection : IProjection, new();
        IDomainEventListener CreateProjectionListener<TProjection>()
            where TProjection : IProjection, new();
    }
}
