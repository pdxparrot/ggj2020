using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.World;

using UnityEngine;

namespace pdxpartyparrot.Game.Effects
{
    public sealed class WorldZoneEffect : MonoBehaviour
    {
        private readonly HashSet<WorldZone> _zones = new HashSet<WorldZone>();

        [SerializeField]
        [ReadOnly]
        private Transform _particleSystemParent;

        public Transform ParticleSystemParent
        {
            get => _particleSystemParent;
            set => _particleSystemParent = value;
        }

#region Unity Lifecycle
        private void OnDestroy()
        {
            foreach(WorldZone zone in _zones) {
                zone.Exit(gameObject);
            }
            _zones.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            WorldZone zone = other.GetComponent<WorldZone>();
            if(null == zone) {
                return;
            }

            zone.Enter(gameObject, ParticleSystemParent);
            _zones.Add(zone);
        }

        private void OnTriggerExit(Collider other)
        {
            WorldZone zone = other.GetComponent<WorldZone>();
            if(null == zone) {
                return;
            }

            zone.Exit(gameObject);
            _zones.Remove(zone);
        }
#endregion
    }
}
