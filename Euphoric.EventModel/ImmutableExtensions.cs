using System;
using System.Collections.Immutable;

namespace BlazorEventsTodo.EventStorage
{
    public static class ImmutableExtensions
    {
        public static ImmutableDictionary<TKey, TValue> Update<TKey, TValue>(this ImmutableDictionary<TKey, TValue> dict, TKey key, Func<TValue, TValue> update)
        {
            var prevValue = dict[key];
            var newValue = update(prevValue);

            return dict.SetItem(key, newValue);
        }
    }
}
