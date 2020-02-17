using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    // TODO: make this an actor
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
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
        private EffectTrigger _holdEffect;

        [SerializeField]
        private SpineAttachmentEffectTriggerComponent _holdAttachmentEffectComponent;

        [SerializeField]
        private EffectTrigger _dropEffect;

        [SerializeField]
        private SpineAttachmentEffectTriggerComponent _dropAttachmentEffectComponent;

        [SerializeField]
        private EffectTrigger _useEffect;

        protected EffectTrigger UseEffect => _useEffect;

        [SerializeField]
        [CanBeNull]
        private RumbleEffectTriggerComponent _useRumbleEffectTriggerComponent;
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

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0.0f;

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

        protected virtual void SetHoldingPlayer([CanBeNull] MechanicBehavior player)
        {
            _holdingPlayer = player;

            if(null != _holdingPlayer) {
                _holdAttachmentEffectComponent.SpineSkinHelper = _holdingPlayer.Owner.Behavior.SpineSkinHelper;
                _dropAttachmentEffectComponent.SpineSkinHelper = _holdingPlayer.Owner.Behavior.SpineSkinHelper;

                if(null != _useRumbleEffectTriggerComponent) {
                    _useRumbleEffectTriggerComponent.PlayerInput = _holdingPlayer.Owner.GamePlayerInput.InputHelper;
                }
            } else {
                _holdAttachmentEffectComponent.SpineSkinHelper = null;
                _dropAttachmentEffectComponent.SpineSkinHelper = null;

                if(null != _useRumbleEffectTriggerComponent) {
                    _useRumbleEffectTriggerComponent.PlayerInput = null;
                }
            }
        }

        public bool Hold(MechanicBehavior player)
        {
            if(IsHeld) {
                return false;
            }

            SetHoldingPlayer(player);

            _holdEffect.Trigger();

            return true;
        }

        public virtual void Drop()
        {
            if(!IsHeld) {
                return;
            }

            SetRepairPoint(null);

            _dropEffect.Trigger();

            GameManager.Instance.GameLevelHelper.ReclaimTool(this);

            SetHoldingPlayer(null);

            _useEffect.StopTrigger();
            _useEffect.gameObject.SetActive(false);
        }

        public bool CanRepair(RepairPoint repairPoint)
        {
            return null != repairPoint && repairPoint.CurrentDamageType == DamageType;
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
                HoldingPlayer.UIBubble.HideSprite();
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
