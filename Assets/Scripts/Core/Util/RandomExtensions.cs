using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace pdxpartyparrot.Core.Util
{
    public static class RandomExtensions
    {
#region Random Collection Entries
        [CanBeNull]
        public static T GetRandomEntry<T>(this Random random, IReadOnlyCollection<T> collection)
        {
            if(collection.Count < 1) {
                return default;
            }

            int idx = random.Next(collection.Count);
            return collection.ElementAt(idx);
        }

        [CanBeNull]
        public static T RemoveRandomEntry<T>(this Random random, IList<T> collection)
        {
            if(collection.Count < 1) {
                return default;
            }

            int idx = random.Next(collection.Count);
            T v = collection.ElementAt(idx);
            collection.RemoveAt(idx);
            return v;
        }
#endregion

        public static int CoinFlip(this Random random)
        {
            return random.NextBool() ? 1 : 0;
        }

        public static float NextSign(this Random random)
        {
            return random.NextBool() ? 1 : -1;
        }

        public static bool NextBool(this Random random)
        {
            return random.Next(2) != 0;
        }

        public static float NextSingle(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextSingle(this Random random, float minValue, float maxValue)
        {
            return (float)random.NextDouble(minValue, maxValue);
        }

        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return minValue + random.NextDouble() * (maxValue - minValue);
        }
    }
}
