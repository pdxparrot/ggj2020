using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.NPCs
{
    [Serializable]
    public class SpawnGroupData
    {
        [Serializable]
        public class ReorderableList : ReorderableList<SpawnGroupData>
        {
        }

        [SerializeField]
        private Actor _actorPrefab;

        public Actor ActorPrefab => _actorPrefab;

        [SerializeField]
        private NPCBehaviorData _npcBehaviorData;

        public NPCBehaviorData NPCBehaviorData => _npcBehaviorData;

        [SerializeField]
        [Tooltip("The spawnpoint tag")]
        private string _tag;
   
        public string Tag => _tag;

        [Space(10)]

        [SerializeField]
        [Tooltip("Time between spawns, in seconds")]
        private IntRangeConfig _delay = new IntRangeConfig(1, 1);

        public IntRangeConfig Delay => _delay;

        [SerializeField]
        [Tooltip("How many actors to spawn each time we spawn")]
        private IntRangeConfig _count = new IntRangeConfig(1, 1);

        public IntRangeConfig Count => _count;

        [Space(10)]

        [SerializeField]
        [Tooltip("How many objects to pre-allocate in the object pool (for pooled objects)")]
        private int _poolSize = 1;

        public int PoolSize => _poolSize;

        [Space(10)]

        [SerializeField]
        [Tooltip("Should we only spawn the wave once?")]
        private bool _once;

        public bool Once => _once;
    }
}
