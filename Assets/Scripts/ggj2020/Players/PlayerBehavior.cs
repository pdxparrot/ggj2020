using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.ggj2020.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    [RequireComponent(typeof(MechanicBehavior))]
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior
    {
        public PlayerBehaviorData GamePlayerBehaviorData => (PlayerBehaviorData)PlayerBehaviorData;

        public Player GamePlayerOwner => (Player)Owner;

        [Space(10)]

#region Effects
        [Header("Player Effects")]

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
                                                ? (GamePlayerOwner.Mechanic.IsHoldingTool ? _climbWithToolEffectTrigger : _climbLadderEffectTrigger)
                                                : (GamePlayerOwner.Mechanic.IsHoldingTool ? _idleWithToolEffect : base.IdleEffect);

        protected override EffectTrigger MovingEffectTrigger => GamePlayerOwner.Mechanic.IsOnLadder
                                                        ? (GamePlayerOwner.Mechanic.IsHoldingTool ? _climbWithToolEffectTrigger : _climbLadderEffectTrigger)
                                                        : (GamePlayerOwner.Mechanic.IsHoldingTool ? _runWithToolEffect : base.MovingEffectTrigger);
#endregion

        public override bool CanMove => base.CanMove && !GamePlayerOwner.Mechanic.IsUsingTool && !GamePlayerOwner.Mechanic.IsUsingChargingStation;

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(Owner is Player);
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);
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
    }
}
