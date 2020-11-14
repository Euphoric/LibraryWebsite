using System;
using System.Reflection;

namespace BlazorEventsTodo.EventStorage
{
    public class PrefixedGuidKey<TKey>
    {
        private readonly string _prefix;
        private readonly ConstructorInfo _constructor;

        public PrefixedGuidKey(string prefix)
        {
            _prefix = prefix + "-";
            _constructor = typeof(TKey).GetConstructor(new[] { typeof(Guid) });
        }

        public string ToValue(Guid id)
        {
            return _prefix + id;
        }

        public TKey Parse(string value)
        {
            if (!value.StartsWith(_prefix))
            {
                throw new Exception("Invalid aggregate key format");
            }
            var guidId = value.Substring(_prefix.Length);

            var guid = Guid.Parse(guidId);
            return (TKey)_constructor.Invoke(new object[] { guid });
        }

        public TKey New()
        {
            var guid = Guid.NewGuid();
            return (TKey)_constructor.Invoke(new object[] { guid });
        }
    }
}
