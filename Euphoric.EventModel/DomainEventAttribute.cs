using System;

namespace BlazorEventsTodo.EventStorage
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
