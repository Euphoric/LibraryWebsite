using System;
using System.Collections.Generic;

namespace Euphoric.EventModel
{
    public class DomainEventSender
    {
        private IEnumerable<IDomainEventListener> _domainEventListeners;

        public DomainEventSender(IEnumerable<IDomainEventListener> domainEventListeners)
        {
            _domainEventListeners = domainEventListeners;
        }

        public void SendEvent(IDomainEvent<IDomainEventData> evnt)
        {
            foreach (var listener in _domainEventListeners)
            {
                listener.Handle(evnt);
            }
        }
    }
}
