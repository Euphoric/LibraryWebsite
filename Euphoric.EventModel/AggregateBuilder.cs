using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Euphoric.EventModel
{
    public record AggregateBuilder<TAggregate>
        where TAggregate : Aggregate
    {
        // ReSharper disable StaticMemberInGenericType
        // This is intended design, these methods are different for each aggregate type
        private static readonly MethodInfo InitializeMethod;
        private static readonly MethodInfo UpdateMethod;
        // ReSharper restore StaticMemberInGenericType
        
        static AggregateBuilder()
        {
            Type aggrType = typeof(TAggregate);
            var initializeMethod = aggrType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            if (initializeMethod == null)
            {
                throw new Exception("Aggregate must have static Initialize method.");
            }
            var updateMethod = aggrType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
            if (updateMethod == null)
            {
                throw new Exception("Aggregate must have Update method.");
            }

            InitializeMethod = initializeMethod;
            UpdateMethod = updateMethod;
        }

        public AggregateBuilder(TAggregate? aggregate)
        {
            Aggregate = aggregate;
        }

        public TAggregate? Aggregate {get; private init; }

        public AggregateBuilder<TAggregate> Apply(IDomainEvent<IDomainEventData> evnt)
        {
            TAggregate? newAggr;
            if (Aggregate == null)
            {
                newAggr = (TAggregate?)InitializeMethod.Invoke(null, new object[] { evnt });
            }
            else
            {
                newAggr = (TAggregate?)UpdateMethod.Invoke(Aggregate, new object[] { evnt });
            }

            if (newAggr == null)
            {
                throw new NullReferenceException("Aggregate returned by Aggregate's Initialize or Update cannot be null.");
            }

            newAggr = newAggr with { Version = evnt.Version, Events = newAggr.Events.Add(evnt) };

            return this with { Aggregate = newAggr };
        }

        public AggregateBuilder<TAggregate> Apply(IEnumerable<IDomainEvent<IDomainEventData>> events)
        {
            return events.Aggregate(this, (builder, evnt) => builder.Apply(evnt));
        }

        public static TAggregate Update(TAggregate? aggregate, IEnumerable<IDomainEvent<IDomainEventData>> events)
        {
            return new AggregateBuilder<TAggregate>(aggregate).Apply(events).Aggregate!;
        }

        public static TAggregate Rehydrate(IEnumerable<IDomainEvent<IDomainEventData>> events)
        {
            return Update(null, events);
        }
    }

    public static class AggregateBuilderExtensions
    {
        public static TAggregate Update<TAggregate>(this TAggregate aggregate, IEnumerable<IDomainEvent<IDomainEventData>> events)
            where TAggregate : Aggregate
        {
            return AggregateBuilder<TAggregate>.Update(aggregate, events);
        }
    }
}
