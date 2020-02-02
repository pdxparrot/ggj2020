using System;

using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2020.Data.Players;
using pdxpartyparrot.ggj2020.World;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior
    {
        public PlayerBehaviorData GamePlayerBehaviorData => (PlayerBehaviorData)PlayerBehaviorData;

        public Player GamePlayerOwner => (Player)Owner;

        [SerializeField]
        private EffectTrigger _robotImpuleEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _rumbleEffect;

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
            set
            {
                _isOnLadder = value;
                Owner.Movement.IsKinematic = value;
            }
        }

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

        public override bool TriggerEnter(GameObject triggerObject)
        {
            Ladder ladder = triggerObject.GetComponent<Ladder>();
            if(null == ladder) {
                return false;
            }

            CanUseLadder = true;

            return false;
        }

        public override bool TriggerExit(GameObject triggerObject)
        {
            Ladder ladder = triggerObject.GetComponent<Ladder>();
            if(null == ladder) {
                return false;
            }

            CanUseLadder = false;

            return false;
        }

        private void RobotImpulseEventHandler(object sender, EventArgs args)
        {
            _robotImpuleEffectTrigger.Trigger();
        }
#endregion
    }
}
