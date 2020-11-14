using System;

namespace Euphoric.EventModel
{
    public interface IProjectionContainerFactory
    {
        IProjectionState<TProjection> CreateProjectionState<TProjection>()
            where TProjection : IProjection, new();
        IDomainEventListener CreateProjectionListener<TProjection>()
            where TProjection : IProjection, new();
    }
}
