using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
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
        [CanBeNull]
        private MechanicBehavior _holdingPlayer;

        [CanBeNull]
        protected MechanicBehavior HoldingPlayer => _holdingPlayer;

        public bool IsHeld => HoldingPlayer != null;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private RepairPoint _repairPoint;

        [CanBeNull]
        public RepairPoint RepairPoint => _repairPoint;

        public bool HasRepairPoint => null != _repairPoint;

        public bool CanUse => IsHeld && null != _repairPoint && CanRepair(_repairPoint);

        [SerializeField]
        [ReadOnly]
        private bool _inUse;

        public bool InUse => _inUse;

        private Rigidbody _rigidbody;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            _useEffect.gameObject.SetActive(false);

            GameManager.Instance.MechanicsCanInteractEvent += MechanicsCanInteractEventHandler;
        }

        protected virtual void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.MechanicsCanInteractEvent -= MechanicsCanInteractEventHandler;
            }
        }
#endregion

        public bool Hold(MechanicBehavior player)
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

        public void Drop()
        {
            if(!IsHeld) {
                return;
            }

            SetRepairPoint(null);

            RemoveAttachment();

            GameManager.Instance.GameLevelHelper.ReclaimTool(this);

            _model.SetActive(true);

            _rigidbody.isKinematic = false;

            _holdingPlayer = null;

            _useEffect.StopTrigger();
            _useEffect.gameObject.SetActive(false);
        }

        public bool CanRepair(RepairPoint repairPoint)
        {
            return null != repairPoint && repairPoint.RepairPointDamageType == DamageType;
        }

        public virtual bool SetRepairPoint(RepairPoint repairPoint)
        {
            if(repairPoint == RepairPoint) {
                return true;
            }

            if(null != RepairPoint) {
                RepairPoint.RepairedEvent -= RepairPointRepairedEventHandler;
            }

            _repairPoint = repairPoint;

            if(null != RepairPoint) {
                _repairPoint.RepairedEvent += RepairPointRepairedEventHandler;

                if(HoldingPlayer.CanUseTool) {
                    ShowBubble();
                }
            } else {
                HoldingPlayer.Owner.UIBubble.HideSprite();
            }

            return true;
        }

        public abstract void ShowBubble();

        public virtual bool Use()
        {
            if(!IsHeld || !HoldingPlayer.CanUseTool) {
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

        public virtual void TrackThumbStickAxis(Vector2 axis)
        {
        }

        protected virtual void OnUseToolEffectEnd()
        {
        }

#region Attachments
        public abstract void SetAttachment();

        public abstract void RemoveAttachment();
#endregion

#region Events
        private void MechanicsCanInteractEventHandler(object sender, EventArgs args)
        {
            if(IsHeld && HoldingPlayer.CanUseTool) {
                ShowBubble();
            }
        }

        private void RepairPointRepairedEventHandler(object sender, EventArgs args)
        {
            SetRepairPoint(null);

            HoldingPlayer.RepairPointRepaired();
        }
#endregion
    }
}
