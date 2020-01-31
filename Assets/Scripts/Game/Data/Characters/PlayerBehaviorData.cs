using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters
{
    [Serializable]
    public abstract class PlayerBehaviorData : CharacterBehaviorData
    {
        [Space(10)]

#region Movement
        [Header("Player Movement")]

        [SerializeField]
        private bool _alignMovementWithViewer;

        public bool AlignMovementWithViewer => _alignMovementWithViewer;
#endregion
    }
}
