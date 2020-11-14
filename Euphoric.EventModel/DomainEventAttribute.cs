using System;

namespace Euphoric.EventModel
{
    public class DomainEventAttribute : Attribute
    {
        public DomainEventAttribute(string eventType)
        {
            EventType = eventType;
        }

        public string EventType { get; }
    }
}
