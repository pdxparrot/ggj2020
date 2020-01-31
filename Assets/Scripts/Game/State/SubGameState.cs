using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public abstract class SubGameState : MonoBehaviour
    {
        [SerializeField]
        [CanBeNull]
        private EffectTrigger _enterEffect;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _exitEffect;

        public string Name => name;

        public virtual void OnEnter()
        {
            Debug.Log($"Enter SubState: {Name}");

            if(null != _enterEffect) {
                _enterEffect.Trigger(DoEnter);
            } else {
                DoEnter();
            }
        }

        // called after enter effects
        protected virtual void DoEnter()
        {
        }

        public virtual void OnExit()
        {
            Debug.Log($"Exit SubState: {Name}");

            // make sure the enter effect is stopped
            if(null != _enterEffect) {
                _enterEffect.KillTrigger();
            }

            if(null != _exitEffect) {
                _exitEffect.Trigger(DoExit);
            } else {
                DoExit();
            }
        }

        // called after exit effects
        protected virtual void DoExit()
        {
        }

        public virtual void OnResume()
        {
            Debug.Log($"Resume SubState: {Name}");
        }

        public virtual void OnPause()
        {
            Debug.Log($"Pause SubState: {Name}");
        }

        public virtual void OnUpdate(float dt)
        {
        }
    }
}
