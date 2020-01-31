using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Collections
{
    public static class CollectionExtensions
    {
        [CanBeNull]
        public static T GetRandomEntry<T>(this IReadOnlyCollection<T> collection)
        {
            return PartyParrotManager.Instance.Random.GetRandomEntry(collection);
        }

        [CanBeNull]
        public static T RemoveRandomEntry<T>(this IList<T> collection)
        {
            return PartyParrotManager.Instance.Random.RemoveRandomEntry(collection);
        }

        public static T PeakFront<T>(this IList<T> list)
        {
            return list.Count < 1 ? default : list[0];
        }

        public static T RemoveFront<T>(this IList<T> list)
        {
            if(list.Count < 1) {
                return default;
            }

            T element = list[0];
            list.RemoveAt(0);
            return element;
        }

        [CanBeNull]
        public static T Nearest<T>(this IReadOnlyCollection<T> collection, Vector3 position, out float distance) where T: Component
        {
            int bestIdx = -1;
            distance = float.PositiveInfinity;

            for(int i=0; i<collection.Count; ++i) {
                T element = collection.ElementAt(i);
                Vector3 epos = element.transform.position;

                float dist = Vector3.Distance(epos, position);
                if(dist < distance) {
                    distance = dist;
                    bestIdx = i;
                }
            }

            return bestIdx < 0 ? null : collection.ElementAt(bestIdx);
        }

        [CanBeNull]
        public static T NearestManhattan<T>(this IReadOnlyCollection<T> collection, Vector3 position, out float distance) where T: Component
        {
            int bestIdx = -1;
            distance = float.PositiveInfinity;

            for(int i=0; i<collection.Count; ++i) {
                T element = collection.ElementAt(i);
                Vector3 epos = element.transform.position;

                float dist = Mathf.Abs(epos.x - position.x) + Mathf.Abs(epos.y - position.y) + Mathf.Abs(epos.z - position.z);
                if(dist < distance) {
                    distance = dist;
                    bestIdx = i;
                }
            }

            return bestIdx < 0 ? null : collection.ElementAt(bestIdx);
        }

        [CanBeNull]
        public static T Furthest<T>(this IReadOnlyCollection<T> collection, Vector3 position, out float distance) where T: Component
        {
            int bestIdx = -1;
            distance = float.NegativeInfinity;

            for(int i=0; i<collection.Count; ++i) {
                T element = collection.ElementAt(i);
                Vector3 epos = element.transform.position;

                float dist = Vector3.Distance(epos, position);
                if(dist > distance) {
                    distance = dist;
                    bestIdx = i;
                }
            }

            return bestIdx < 0 ? null : collection.ElementAt(bestIdx);
        }

        [CanBeNull]
        public static T FurthestManhattan<T>(this IReadOnlyCollection<T> collection, Vector3 position, out float distance) where T: Component
        {
            int bestIdx = -1;
            distance = float.NegativeInfinity;

            for(int i=0; i<collection.Count; ++i) {
                T element = collection.ElementAt(i);
                Vector3 epos = element.transform.position;

                float dist = Mathf.Abs(epos.x - position.x) + Mathf.Abs(epos.y - position.y) + Mathf.Abs(epos.z - position.z);
                if(dist > distance) {
                    distance = dist;
                    bestIdx = i;
                }
            }

            return bestIdx < 0 ? null : collection.ElementAt(bestIdx);
        }

        public static void WithinDistance<T>(this IReadOnlyCollection<T> collection, Vector3 position, float distance, IList<T> matches) where T: Component
        {
            foreach(T element in collection) {
                Vector3 epos = element.transform.position;

                float dist = Mathf.Abs(epos.x - position.x) + Mathf.Abs(epos.y - position.y) + Mathf.Abs(epos.z - position.z);
                if(dist <= distance) {
                    matches.Add(element);
                }
            }
        }
    }
}
