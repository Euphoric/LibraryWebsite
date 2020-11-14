using System;

namespace Euphoric.EventModel
{
    public interface IDomainEventListener
    {
        void Handle(IDomainEvent<IDomainEventData> evnt);
    }
}
