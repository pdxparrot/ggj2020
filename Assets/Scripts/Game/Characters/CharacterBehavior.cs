using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Actors.Components;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Data.Characters;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters
{
    // TODO: would composition make more sense than inheritance here?
    public abstract class CharacterBehavior : ActorBehaviorComponent
    {
        private struct ActionBufferEntry
        {
            public CharacterBehaviorComponent.CharacterBehaviorAction Action { get; }

            public ActionBufferEntry(CharacterBehaviorComponent.CharacterBehaviorAction action)
            {
                Action = action;
            }

            public override string ToString()
            {
                return $"{Action}";
            }
        }

        [CanBeNull]
        public ICharacterMovement CharacterMovement => (ICharacterMovement)Owner.Movement;

        [CanBeNull]
        public CharacterBehaviorData CharacterBehaviorData => (CharacterBehaviorData)BehaviorData;

        [Header("Components")]

        [SerializeField]
        [ReorderableList]
        private CharacterBehaviorComponent.ReorderableList _components = new CharacterBehaviorComponent.ReorderableList();

        [Space(10)]

#region Physics
        [Header("Character Physics")]

        [SerializeField]
        [ReadOnly]
        private bool _isGrounded;

        public bool IsGrounded
        {
            get => _isGrounded;
            set => _isGrounded = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _isSliding;

        public bool IsSliding
        {
            get => _isSliding;
            set => _isSliding = value;
        }

        public bool IsFalling => Owner.Movement.UseGravity && (!IsGrounded && !IsSliding && Owner.Movement.Velocity.y < 0.0f);

        public virtual float MoveSpeed => CharacterBehaviorData.MoveSpeed;
#endregion

        [Space(10)]

#region Effects
        [Header("Character Effects")]

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _idleEffect;

        [CanBeNull]
        protected virtual EffectTrigger IdleEffect => _idleEffect;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _movingEffectTrigger;

        [CanBeNull]
        protected virtual EffectTrigger MovingEffectTrigger => _movingEffectTrigger;
#endregion

        public override bool CanMove => !CharacterMovement.IsComponentControlling && CanMoveNoComponents;

        private bool CanMoveNoComponents
        {
            get
            {
                // TODO: make this configurable
                if(PartyParrotManager.Instance.IsPaused || !GameStateManager.Instance.GameManager.IsGameReady || GameStateManager.Instance.GameManager.IsGameOver) {
                    return false;
                }

                return true;
            }
        }

#region Action Buffer
        [CanBeNull]
        private CircularBuffer<ActionBufferEntry> _actionBuffer;

        public CharacterBehaviorComponent.CharacterBehaviorAction NextAction => null == _actionBuffer || _actionBuffer.Count < 1 ? null : _actionBuffer.Head.Action;
#endregion

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }

        protected override void Update()
        {
            base.Update();

            if(null != Animator) {
                Animator.SetBool(CharacterBehaviorData.FallingParam, IsFalling);
            }
        }
#endregion

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(behaviorData is CharacterBehaviorData);

            base.Initialize(behaviorData);

            _actionBuffer = new CircularBuffer<ActionBufferEntry>(CharacterBehaviorData.ActionBufferSize);

            foreach(CharacterBehaviorComponent component in _components.Items) {
                component.Initialize(this);
            }

            InitDebugMenu();
        }

#region Components
        [CanBeNull]
        public T GetBehaviorComponent<T>() where T: CharacterBehaviorComponent
        {
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    return tc;
                }
            }
            return null;
        }

        public void GetBehaviorComponents<T>(ICollection<T> components) where T: CharacterBehaviorComponent
        {
            components.Clear();
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    components.Add(tc);
                }
            }
        }

        public void RunOnComponents(Func<CharacterBehaviorComponent, bool> f)
        {
            foreach(CharacterBehaviorComponent component in _components.Items) {
                if(f(component)) {
                    return;
                }
            }
        }
#endregion

#region Action Buffer
        public void BufferAction(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            _actionBuffer?.Add(new ActionBufferEntry(action));
        }

        public void PopNextAction()
        {
            _actionBuffer?.RemoveOldest();
        }

        public void ClearActionBuffer()
        {
            _actionBuffer?.Clear();
        }
#endregion

#region Actions
        public virtual void ActionStarted(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            RunOnComponents(c => c.OnStarted(action));
        }

        public virtual void ActionPerformed(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            RunOnComponents(c => c.OnPerformed(action));
        }

        public virtual void ActionCancelled(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            RunOnComponents(c => c.OnCancelled(action));
        }
#endregion

        protected override void AnimationUpdate(float dt)
        {
            if(!CanMoveNoComponents) {
                return;
            }

            RunOnComponents(c => c.OnAnimationUpdate(dt));
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMoveNoComponents) {
                return;
            }

            RunOnComponents(c => c.OnPhysicsUpdate(dt));
        }

        protected void AlignToMovement(Vector3 forward)
        {
            if(!Owner.IsMoving) {
                return;
            }

            Owner.SetFacing(forward);
        }

        protected virtual void TriggerMoveEffect()
        {
            // TODO: split moving into walking / running
            if(Owner.IsMoving && null != MovingEffectTrigger) {
                MovingEffectTrigger.Trigger();
            } else if(!Owner.IsMoving && null != IdleEffect) {
                IdleEffect.Trigger();
            }
        }

#region Events
        public virtual void OnIdle()
        {
            TriggerMoveEffect();
        }

        protected override void OnSpawnComplete()
        {
            base.OnSpawnComplete();

            OnIdle();
        }

        protected override void OnReSpawnComplete()
        {
            base.OnReSpawnComplete();

            OnIdle();
        }

        public override bool OnMoveStateChanged()
        {
            base.OnMoveStateChanged();

            TriggerMoveEffect();

            return false;
        }
#endregion

#region Debug Menu
        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => $"Game.CharacterBehavior {Owner.Id}");
            _debugMenuNode.RenderContentsAction = () => {
                if(null != _actionBuffer) {
                    GUILayout.BeginVertical("Action Buffer", GUI.skin.box);
                        foreach(var action in _actionBuffer) {
                            GUILayout.Label(action.ToString());
                        }
                    GUILayout.EndVertical();
                }
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
#endregion
    }
}
