using System;
using System.Collections.Generic;

namespace BlazorEventsTodo.EventStorage
{
    public class SynchronousProjectionContainerFactory : IProjectionContainerFactory
    {
        Dictionary<Type, object> _projectionContainers = new Dictionary<Type, object>();

        private ProjectionContainer<TProjection> CreateProjection<TProjection>()
            where TProjection : IProjection, new() 
        {
            if (_projectionContainers.TryGetValue(typeof(TProjection), out object projection))
            {
                return (ProjectionContainer<TProjection>)projection;
            }

            ProjectionContainer<TProjection> container = new ProjectionContainer<TProjection>();
            _projectionContainers[typeof(TProjection)] = container;
            return container;
        }

        public IDomainEventListener CreateProjectionListener<TProjection>()
            where TProjection : IProjection, new()
        {
            return CreateProjection<TProjection>();
        }

        public IProjectionState<TProjection> CreateProjectionState<TProjection>()
            where TProjection : IProjection, new()
        {
            return CreateProjection<TProjection>();
        }
    }

    public class ProjectionContainer<TProjection> : IDomainEventListener, IProjectionState<TProjection>
        where TProjection : IProjection, new()
    {
        IProjection _state = new TProjection();
        public TProjection State { get => (TProjection)_state; }

        public void Handle(IDomainEvent<IDomainEventData> evnt)
        {
            _state = State.NextState(evnt);
        }
    }
}
