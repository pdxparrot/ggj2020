using System.Collections.Generic;

using JetBrains.Annotations;

namespace pdxpartyparrot.Core.Collections
{
    public static class DictionaryExtensions
    {
        [CanBeNull]
        public static TV GetOrDefault<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key, TV defaultValue=default)
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static TV GetOrAdd<TK, TV>(this Dictionary<TK, TV> dict, TK key) where TV: new()
        {
            if(dict.TryGetValue(key, out var value)) {
                return value;
            }

            value = new TV();
            dict.Add(key, value);
            return value;
        }

        public static bool Remove<TK, TV>(this IDictionary<TK, TV> dict, TK key, out TV value)
        {
            return dict.TryGetValue(key, out value) && dict.Remove(key);
        }
    }
}
