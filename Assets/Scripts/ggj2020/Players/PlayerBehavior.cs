using System;

using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.ggj2020.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    [RequireComponent(typeof(Mechanic))]
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior
    {
        public PlayerBehaviorData GamePlayerBehaviorData => (PlayerBehaviorData)PlayerBehaviorData;

        public Player GamePlayerOwner => (Player)Owner;

        [Space(10)]

        [SerializeField]
        private EffectTrigger _robotImpuleEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _rumbleEffect;

        [SerializeField]
        private EffectTrigger _idleWithToolEffect;

        [SerializeField]
        private EffectTrigger _runWithToolEffect;

        [SerializeField]
        private EffectTrigger _climbLadderEffectTrigger;

        public EffectTrigger ClimbLadderEffectTrigger => _climbLadderEffectTrigger;

        [SerializeField]
        private EffectTrigger _climbWithToolEffectTrigger;

        protected override EffectTrigger IdleEffect => GamePlayerOwner.Mechanic.IsOnLadder
                                                ? (GamePlayerOwner.Mechanic.HasTool ? _climbWithToolEffectTrigger : _climbLadderEffectTrigger)
                                                : (GamePlayerOwner.Mechanic.HasTool ? _idleWithToolEffect : base.IdleEffect);

        protected override EffectTrigger MovingEffectTrigger => GamePlayerOwner.Mechanic.IsOnLadder
                                                        ? (GamePlayerOwner.Mechanic.HasTool ? _climbWithToolEffectTrigger : _climbLadderEffectTrigger)
                                                        : (GamePlayerOwner.Mechanic.HasTool ? _runWithToolEffect : base.MovingEffectTrigger);

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            GameManager.Instance.RobotImpulseEvent += RobotImpulseEventHandler;
        }

        protected override void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.RobotImpulseEvent -= RobotImpulseEventHandler;
            }

            base.OnDestroy();
        }
#endregion

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(Owner is Player);
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);

            _rumbleEffect.PlayerInput = GamePlayerOwner.GamePlayerInput.InputHelper;
        }

        protected override void PhysicsUpdate(float dt)
        {
            base.PhysicsUpdate(dt);

            // hack to prevent ladder movement going through the floor
            if(Owner.Movement.Position.y < 0.0f) {
                Vector3 pos = Owner.Movement.Position;
                pos.y = 0.0f;
                Owner.Movement.Teleport(pos);
            }
        }

#region Events
        public override bool OnDeSpawn()
        {
            _robotImpuleEffectTrigger.StopTrigger();

            return base.OnDeSpawn();
        }

        private void RobotImpulseEventHandler(object sender, EventArgs args)
        {
            _robotImpuleEffectTrigger.Trigger();
        }
#endregion
    }
}
