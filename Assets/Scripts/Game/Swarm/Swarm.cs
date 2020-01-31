using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;

namespace pdxpartyparrot.Game.Swarm
{
    public class Swarm : MonoBehaviour
    {
        [SerializeField]
        private float _swarmRadius = 1.0f;

        [SerializeField]
        [CanBeNull]
        private Transform _center;

        public Transform Center => null == _center ? transform : _center;

        private readonly List<ISwarmable> _swarmables = new List<ISwarmable>();

        public bool HasSwarm => _swarmables.Count > 0;

        public void Add(ISwarmable swarmable)
        {
            if(!swarmable.CanJoinSwarm) {
                return;
            }

            _swarmables.Add(swarmable);
            swarmable.JoinSwarm(this, _swarmRadius);
        }

        public int Remove(int amount)
        {
            int removed = 0;
            for(int i=0; i<_swarmables.Count && removed < amount; ++i) {
                ISwarmable swarmable = _swarmables[i];
                swarmable.RemoveFromSwarm();

                removed++;
            }
            _swarmables.RemoveRange(0, removed);

            return removed;
        }

        public void RemoveAll()
        {
            foreach(ISwarmable swarmable in _swarmables) {
                swarmable.RemoveFromSwarm();
            }
            _swarmables.Clear();
        }
    }
}
