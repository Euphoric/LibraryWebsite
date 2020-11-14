using System;

namespace Euphoric.EventModel
{
    public interface IAggregateKey<TAggregate, TEvent>
        where TAggregate: Aggregate
        where TEvent: IDomainEventData
    {
        string Value { get; }
    }
}
