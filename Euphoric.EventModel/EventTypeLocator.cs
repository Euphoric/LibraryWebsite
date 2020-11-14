using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazorEventsTodo.EventStorage
{
    public class EventTypeLocator
    {
        private Dictionary<Type, string> _typeToString;
        private Dictionary<string, Type> _stringToType;

        public EventTypeLocator()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Select(tp => new { type=tp, attr= tp.GetCustomAttribute<DomainEventAttribute>() } ).Where(x=>x.attr != null);

            _typeToString = types.ToDictionary(x=>x.type, x=>x.attr.EventType);
            _stringToType = types.ToDictionary(x=>x.attr.EventType, x=>x.type);
        }

        internal string GetTypeString(Type eventType)
        {
            return _typeToString[eventType];
        }

        internal Type GetClrType(string eventType)
        {
            return _stringToType.GetValueOrDefault(eventType);
        }
    }
}
