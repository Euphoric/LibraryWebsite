using System.Collections.Immutable;

namespace BlazorEventsTodo.EventStorage
{
    public record Aggregate
    {
        public ulong Version { get; init; } = 0;
        public ImmutableList<IDomainEvent<IDomainEventData>> Events { get; init; } = ImmutableList<IDomainEvent<IDomainEventData>>.Empty;
    };
}
