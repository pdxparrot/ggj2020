using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Actors;
using pdxpartyparrot.ggj2020.Actors.Tools;
using pdxpartyparrot.ggj2020.UI;
using pdxpartyparrot.ggj2020.World;

using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    // TODO: make an ActorComponent
    [RequireComponent(typeof(Interactables3D))]
    public sealed class MechanicBehavior : MonoBehaviour
    {
        [SerializeField]
        private Player _owner;

        public Player Owner => _owner;

        [Space(10)]

#region Ladder
        [SerializeField]
        [ReadOnly]
        private bool _canUseLadder;

        public bool CanUseLadder
        {
            get => _canUseLadder;
            private set
            {
                _canUseLadder = value;
                IsOnLadder = IsOnLadder && _canUseLadder;
            }
        }

        [SerializeField]
        [ReadOnly]
        private bool _isOnLadder;

        public bool IsOnLadder
        {
            get => _isOnLadder;
            private set
            {
                _isOnLadder = value;
                Owner.Movement.IsKinematic = value;
            }
        }
#endregion

        [Space(10)]

#region Tool
        [SerializeField]
        private BoneFollower _toolAttachment;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Tool _heldTool;

        public bool HasTool => _heldTool != null;

        public bool IsUsingTool => HasTool && _heldTool.InUse;

        public bool CanUseTool => GameManager.Instance.MechanicsCanInteract && !IsOnLadder && HasTool && _heldTool.CanUse && !IsUsingTool;
#endregion

        [Space(10)]

#region UI
        [Header("UI")]

        [SerializeField]
        private ToolBubble _toolBubble;

        public ToolBubble ToolBubble => _toolBubble;
#endregion

        [Space(10)]

#region Effects
        [Header("Effects")]

        [SerializeField]
        private EffectTrigger _pickupToolEffect;

        [SerializeField]
        private EffectTrigger _dropToolEffect;

        [SerializeField]
        private EffectTrigger _useToolEffect;

        [SerializeField]
        private EffectTrigger _robotImpuleEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _rumbleEffect;
#endregion

        private Interactables _interactables;

#region Unity Lifecycle
        private void Awake()
        {
            _interactables = GetComponent<Interactables>();
            _interactables.InteractableAddedEvent += InteractableAddedEventHandler;
            _interactables.InteractableRemovedEvent += InteractableRemovedEventHandler;

            ToolBubble.Hide();

            GameManager.Instance.MechanicsCanInteractEvent += MechanicsCanInteractEventHandler;
            GameManager.Instance.RobotImpulseEvent += RobotImpulseEventHandler;
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.RobotImpulseEvent -= RobotImpulseEventHandler;
                GameManager.Instance.MechanicsCanInteractEvent -= MechanicsCanInteractEventHandler;
            }
        }
#endregion

        public void Initialize()
        {
            _rumbleEffect.PlayerInput = Owner.GamePlayerInput.InputHelper;
        }

#region Ladder
        public void HandleLadderInput()
        {
            if(IsUsingTool) {
                return;
            }

            if(IsOnLadder) {
                IsOnLadder = false;
            } else if(CanUseLadder) {
                Owner.GamePlayerBehavior.ClimbLadderEffectTrigger.Trigger();
                IsOnLadder = true;
            }
        }
#endregion

#region Tool
        private RepairPoint GetDamagedRepairPoint(RepairPoint.DamageType damageType)
        {
            var interactables = _interactables.GetInteractables<RepairPoint>();
            foreach(IInteractable interactable in interactables) {
                RepairPoint repairPoint = (RepairPoint)interactable;
                if(!repairPoint.IsRepaired && repairPoint.CurrentDamageType == damageType) {
                    return repairPoint;
                }
            }
            return null;
        }

        private void PickupTool()
        {
            if(HasTool) {
                return;
            }

            Tool tool = _interactables.GetRandomInteractable<Tool>();
            if(null == tool) {
                return;
            }

            if(!tool.Hold(this)) {
                return;
            }

            _heldTool = tool;
            _interactables.RemoveInteractable(_heldTool);

            _heldTool.transform.SetParent(_toolAttachment.transform);

            // see if we have a repair already
            RepairPoint repairPoint = GetDamagedRepairPoint(_heldTool.DamageType);
            if(null != repairPoint) {
                _heldTool.SetRepairPoint(repairPoint);
            }

            _pickupToolEffect.Trigger();
        }

        private bool UseTool()
        {
            if(!CanUseTool) {
                return false;
            }

            if(!_heldTool.Use()) {
                return false;
            }

            _useToolEffect.Trigger();
            return true;
        }

        public void DropTool()
        {
            if(!HasTool || IsUsingTool) {
                return;
            }

            _heldTool.Drop();

            _interactables.AddInteractable(_heldTool);
            _heldTool = null;

            ToolBubble.Hide();

            _dropToolEffect.Trigger();
        }

        // returns true if we're using a tool
        public bool HandleToolInput()
        {
            if(IsOnLadder) {
                return false;
            }

            if(UseTool()) {
                return true;
            }

            PickupTool();
            return false;
        }

        public void TrackThumbStickAxis(Vector2 axis)
        {
            if(!IsUsingTool) {
                return;
            }

            _heldTool.TrackThumbStickAxis(axis);
        }

        public void UseEnded()
        {
            if(!IsUsingTool) {
                return;
            }

            _heldTool.EndUse();
            Owner.Behavior.SpineAnimationHelper.SetEmptyAnimation(1);
        }

        public void RepairPointRepaired()
        {
            UseEnded();

            if(HasTool && !_heldTool.HasRepairPoint) {
                RepairPoint repairPoint = GetDamagedRepairPoint(_heldTool.DamageType);
                _heldTool.SetRepairPoint(repairPoint);
            }
        }
#endregion

#region Events
        public bool OnSpawn(SpawnPoint spawnpoint)
        {
            ToolBubble.Hide();

            return true;
        }

        public bool OnDeSpawn()
        {
            _robotImpuleEffectTrigger.StopTrigger();

            return true;
        }

        private void RobotImpulseEventHandler(object sender, EventArgs args)
        {
            _robotImpuleEffectTrigger.Trigger();
        }

        private void MechanicsCanInteractEventHandler(object sender, EventArgs args)
        {
            if(!GameManager.Instance.MechanicsCanInteract) {
                ToolBubble.Hide();
            }
        }

        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            Ladder ladder = args.Interactable as Ladder;
            if(!CanUseLadder && null != ladder) {
                CanUseLadder = true;
                return;
            }

            if(HasTool && !_heldTool.HasRepairPoint) {
                RepairPoint repairPoint = args.Interactable as RepairPoint;
                if(_heldTool.CanRepair(repairPoint)) {
                    _heldTool.SetRepairPoint(repairPoint);
                }
            }
        }

        private void InteractableRemovedEventHandler(object sender, InteractableEventArgs args)
        {
            Ladder ladder = args.Interactable as Ladder;
            if(null != ladder && !_interactables.HasInteractables<Ladder>()) {
                CanUseLadder = false;
                return;
            }

            if(HasTool && _heldTool.HasRepairPoint) {
                RepairPoint repairPoint = args.Interactable as RepairPoint;
                if(null != repairPoint && _heldTool.RepairPoint == repairPoint) {
                    repairPoint = GetDamagedRepairPoint(_heldTool.DamageType);
                    _heldTool.SetRepairPoint(repairPoint);
                }
            }
        }
#endregion
    }
}
