using System;

using pdxpartyparrot.Game.Level;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Level
{
    public class RepairBayLevel : LevelHelper
    {
        [Space(10)]

        [SerializeField]
        private Collider2D _cameraBounds;

#region Events
        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }
#endregion
    }
}
