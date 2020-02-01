using pdxpartyparrot.Core.Data.Actors.Components;
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

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(Owner is Player);
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);
        }

#region Events
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
#endregion
    }
}
