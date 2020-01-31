using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="ViewerBoundsPlayerBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/ViewerBoundsPlayerBehaviorComponent Data")]
    [Serializable]
    public class ViewerBoundsPlayerBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        private bool _constrainX = true;

        public bool ConstrainX => _constrainX;

        [SerializeField]
        private bool _constrainY = true;

        public bool ConstrainY => _constrainY;
    }
}