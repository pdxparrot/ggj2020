using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Actors;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    // TODO: make this an actor
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Tool : MonoBehaviour, IInteractable
    {
        public bool CanInteract => !IsHeld;

        public Type InteractableType => typeof(Tool);

        [SerializeField]
        private GameObject _model;

        [SerializeField]
        private RepairPoint.DamageType _damageType;

        public RepairPoint.DamageType DamageType => _damageType;

        [Space(10)]

#region Effects
        [Header("Effects")]

        [SerializeField]
        private EffectTrigger _useEffect;

        protected EffectTrigger UseEffect => _useEffect;
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private bool _inUse;

        public bool InUse => _inUse;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Mechanic _holdingPlayer;

        [CanBeNull]
        protected Mechanic HoldingPlayer => _holdingPlayer;

        public bool IsHeld => HoldingPlayer != null;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private RepairPoint _repairPoint;

        [CanBeNull]
        protected RepairPoint RepairPoint => _repairPoint;

        private Rigidbody _rigidbody;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            _useEffect.gameObject.SetActive(false);
        }
#endregion

        public virtual bool Hold(Mechanic player)
        {
            if(IsHeld) {
                return false;
            }

            _holdingPlayer = player;

            _rigidbody.isKinematic = true;

            _model.SetActive(false);

            SetAttachment();

            return true;
        }

        public virtual void Drop()
        {
            if(!IsHeld) {
                return;
            }

            RemoveAttachment();

            GameManager.Instance.GameLevelHelper.ReclaimTool(this);

            _model.SetActive(true);

            _rigidbody.isKinematic = false;

            _holdingPlayer = null;

            _useEffect.StopTrigger();
            _useEffect.gameObject.SetActive(false);
        }

        public bool SetRepairPoint(RepairPoint repairPoint)
        {
            if(null != RepairPoint) {
                return false;
            }

            _repairPoint = repairPoint;
            _repairPoint.RepairedEvent += RepairPointRepairedEventHandler;

            return true;
        }

        public virtual void TrackThumbStickAxis(Vector2 axis)
        {
        }

        public abstract void CanUse();

        public virtual bool Use()
        {
            if(!GameManager.Instance.MechanicsCanInteract || !IsHeld || null == _repairPoint) {
                return false;
            }

            _inUse = true;

            _useEffect.gameObject.SetActive(true);
            _useEffect.Trigger(OnUseToolEffectEnd);

            return true;
        }

        public virtual void EndUse()
        {
            _inUse = false;

            _useEffect.StopTrigger();
            _useEffect.gameObject.SetActive(false);
        }

        protected virtual void OnUseToolEffectEnd()
        {
        }

#region Attachments
        public abstract void SetAttachment();

        public abstract void RemoveAttachment();
#endregion

#region Events
        private void RepairPointRepairedEventHandler(object sender, EventArgs args)
        {
            _repairPoint = null;

            HoldingPlayer.UseEnded();
        }
#endregion
    }
}
