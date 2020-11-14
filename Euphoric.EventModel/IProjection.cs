using System;

namespace Euphoric.EventModel
{
    public interface IProjection
    {
        IProjection NextState(IDomainEvent<IDomainEventData> evnt);
    }
}
