using System;
using System.Reflection;

namespace Euphoric.EventModel
{
    public class PrefixedGuidKey<TKey>
    {
        private readonly string _prefix;
        private readonly ConstructorInfo _constructor;

        public PrefixedGuidKey(string prefix)
        {
            _prefix = prefix + "-";
            var constructorInfo = typeof(TKey).GetConstructor(new[] { typeof(Guid) });
            if (constructorInfo == null)
            {
                throw new NullReferenceException("Key must have constructor with Guid parameter.");
            }
            _constructor = constructorInfo;
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
