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
        private EffectTrigger _introEffectTrigger;

        [SerializeField]
        private EffectTrigger _winEffectTrigger;

        [SerializeField]
        private EffectTrigger _loseChargedEffectTrigger;

        [SerializeField]
        private EffectTrigger _loseNoChargeEffectTrigger;
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            // TODO: this should come from an actor data object for this
            Rigidbody.isKinematic = true;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            Model.gameObject.SetActive(false);
        }
#endregion

        public void BeginFight(bool success, bool isCharged)
        {
            _introEffectTrigger.Trigger(() => {
                if(success) {
                    _winEffectTrigger.Trigger();
                } else {
                    if(isCharged) {
                        _loseChargedEffectTrigger.Trigger();
                    } else {
                        _loseNoChargeEffectTrigger.Trigger();
                    }
                }
            });
        }
    }
}
