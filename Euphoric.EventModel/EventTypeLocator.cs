using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Euphoric.EventModel
{
    public class EventTypeLocator
    {
        private readonly Dictionary<Type, string> _typeToString;
        private readonly Dictionary<string, Type> _stringToType;

        public EventTypeLocator(Assembly eventsAssembly)
        {
            var types = eventsAssembly.GetTypes()
                .Select(tp => new { type=tp, attr= tp.GetCustomAttribute<DomainEventAttribute>() } )
                .Where(x=>x.attr != null)
                .Select(x => new { x.type, attr= x.attr! } )
                .ToArray();

            _typeToString = types.ToDictionary(x=>x.type, x=>x.attr.EventType);
            _stringToType = types.ToDictionary(x=>x.attr.EventType, x=>x.type);
        }

        internal string GetTypeString(Type eventType)
        {
            return _typeToString[eventType];
        }

        internal Type? GetClrType(string eventType)
        {
            return _stringToType.GetValueOrDefault(eventType);
        }
    }
}
