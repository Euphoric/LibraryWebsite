using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BlazorEventsTodo.EventStorage
{
    public class ThreadedProjectionContainerFactory : IProjectionContainerFactory
    {
        Dictionary<Type, object> _projectionContainers = new Dictionary<Type, object>();

        private ThreadedProjectionContainer<TProjection> CreateProjection<TProjection>()
            where TProjection : IProjection, new()
        {
            if (_projectionContainers.TryGetValue(typeof(TProjection), out object projection))
            {
                return (ThreadedProjectionContainer<TProjection>)projection;
            }

            ThreadedProjectionContainer<TProjection> container = new ThreadedProjectionContainer<TProjection>();
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

    public class ThreadedProjectionContainer<TProjection> : IDomainEventListener, IProjectionState<TProjection>
        where TProjection : IProjection, new()
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<IDomainEvent<IDomainEventData>> _eventQueue = new BlockingCollection<IDomainEvent<IDomainEventData>>(new ConcurrentQueue<IDomainEvent<IDomainEventData>>());

        IProjection _state = new TProjection();

        public TProjection State { get => (TProjection)_state; }

        public ThreadedProjectionContainer()
        {
            _thread = new Thread(RunOnThread);
            _thread.Start();
        }

        private void RunOnThread()
        {
            while (true)
            {
                var evnt = _eventQueue.Take();
                _state = State.NextState(evnt);
            }
        }

        public void Handle(IDomainEvent<IDomainEventData> evnt)
        {
            _eventQueue.Add(evnt);
        }
    }
}
