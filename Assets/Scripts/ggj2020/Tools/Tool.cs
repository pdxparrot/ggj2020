using System;

using JetBrains.Annotations;

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

        [SerializeField]
        [ReadOnly]
        private bool _inUse;

        protected bool InUse => _inUse;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private RepairPoint _repairPoint;

        [CanBeNull]
        protected RepairPoint RepairPoint
        {
            get => _repairPoint;
            set => _repairPoint = value;
        }

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Mechanic _holdingPlayer;

        [CanBeNull]
        protected Mechanic HoldingPlayer
        {
            get => _holdingPlayer;
            set => _holdingPlayer = value;
        }

        public bool IsHeld => _holdingPlayer != null;

        private Rigidbody _rigidbody;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
#endregion

        public virtual void PlayerExitTrigger()
        {
        }

        public virtual bool SetHeld(Mechanic player)
        {
            if(IsHeld) {
                return false;
            }

            _holdingPlayer = player;

            _rigidbody.isKinematic = true;

            _model.SetActive(false);
            transform.SetParent(player.transform);

            SetAttachment();

            return true;
        }

        public virtual void Drop()
        {
            if(!IsHeld) {
                return;
            }

            RemoveAttachment();

            transform.SetParent(null);
            _model.SetActive(true);

            _rigidbody.isKinematic = false;

            _holdingPlayer.Owner.UIBubble.HideSprite();
            _holdingPlayer = null;
        }

        public virtual void TrackThumbStickAxis(Vector2 axis)
        {
        }

        public virtual bool UseTool()
        {
            if(!GameManager.Instance.MechanicsCanInteract || !IsHeld) {
                return false;
            }

            _inUse = true;

            return true;
        }

        public virtual void EndUseTool()
        {
            _inUse = false;
        }

        public virtual void SetAttachment()
        {
        }

        public virtual void RemoveAttachment()
        {
        }
    }
}
