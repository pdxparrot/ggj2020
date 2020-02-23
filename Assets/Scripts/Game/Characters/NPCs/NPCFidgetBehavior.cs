using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    public class NPCFidgetBehavior : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private INPC _owner;

        // TODO: move this to a config
        [SerializeField]
        private float _cooldownSeconds = 5.0f;

        // TODO: move this to a config
        [SerializeField]
        private FloatRangeConfig _fidgetRange;

        [SerializeField]
        [ReadOnly]
        private Vector3 _origin;

        public Vector3 Origin
        {
            get => _origin;
            set
            {
                _origin = value;

                if(GameStateManager.Instance.NPCManager.DebugBehavior) {
                    Debug.Log($"NPC {_owner.Id} reset fidget origin to {_origin}");
                }
            }
        }

        [SerializeField]
        [ReadOnly]
        private Vector3 _offset;

        [SerializeReference]
        [ReadOnly]
        private ITimer _cooldown;

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsTrue(_fidgetRange.Valid);

            _cooldown = TimeManager.Instance.AddTimer();
        }

        private void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_cooldown);
                _cooldown = null;
            }
        }
#endregion

        public void Initialize(INPC owner)
        {
            _owner = owner;
        }

        public void Fidget()
        {
            if(_cooldown.IsRunning || _owner.IsMoving) {
                return;
            }

            if(GameStateManager.Instance.NPCManager.DebugBehavior) {
                Debug.Log($"NPC {_owner.Id} fidgeting");
            }

            _offset = new Vector3(_fidgetRange.GetRandomValue() * PartyParrotManager.Instance.Random.NextSign(),
                                  0.0f,
                                  _fidgetRange.GetRandomValue() * PartyParrotManager.Instance.Random.NextSign());

            _owner.UpdatePath(Origin + _offset);

            // TODO: really we want to start this when the NPC reaches their destination
            _cooldown.Start(_cooldownSeconds);
        }
    }
}
