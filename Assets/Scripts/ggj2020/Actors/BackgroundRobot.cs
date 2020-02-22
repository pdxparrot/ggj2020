using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    public sealed class BackgroundRobot : Actor3D
    {
        public override bool IsLocalActor => true;

        [Space(10)]

#region Effects
        [SerializeField]
        private EffectTrigger _idleEffectTrigger;

        [SerializeField]
        private EffectTrigger _winEffectTrigger;

        [SerializeField]
        private EffectTrigger _loseEffectTrigger;
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            // TODO: this should come from an actor data object for this
            Rigidbody.isKinematic = true;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
#endregion

        public void Idle()
        {
            _idleEffectTrigger.Trigger();
        }

        public void Win()
        {
            Debug.Log($"Background robot {name} wins!");
            _winEffectTrigger.Trigger(Idle);
        }

        public void Lose()
        {
            Debug.Log($"Background robot {name} loses!");
            _loseEffectTrigger.Trigger(Idle);
        }
    }
}
