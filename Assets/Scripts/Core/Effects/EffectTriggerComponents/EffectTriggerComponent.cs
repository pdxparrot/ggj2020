using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    [RequireComponent(typeof(EffectTrigger))]
    public abstract class EffectTriggerComponent : MonoBehaviour
    {
        [Serializable]
        public class ReorderableList : ReorderableList<EffectTriggerComponent>
        {
        }

        protected  EffectTrigger Owner { get; private set; }

        public abstract bool WaitForComplete { get; }

        public virtual bool IsDone => true;

        public virtual void Initialize(EffectTrigger owner)
        {
            Owner = owner;
        }

        public abstract void OnStart();

        public virtual void OnStop()
        {
        }

        public virtual void OnReset()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }
    }
}
