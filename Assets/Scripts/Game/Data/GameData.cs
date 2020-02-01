using System;

using JetBrains.Annotations;

using pdxpartyparrot.Game.UI;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [Serializable]
    public abstract class GameData : ScriptableObject
    {
        [Header("Viewport")]

        // TODO: this probably isn't the best way to handle this or the best place to put it
        // TODO: also, this is the *2D* viewport size and entirely irrelevant to 3D games
        // and that should be made clearer in the data
        [SerializeField]
        [Tooltip("The orthographic size of the 2D camera, which is also the height of the viewport.")]
        private float _viewportSize = 10;

        public float ViewportSize => _viewportSize;

        [Space(10)]

        [Header("Players")]

        [SerializeField]
        private int _maxLocalPlayers = 1;

        public int MaxLocalPlayers => _maxLocalPlayers;

        [SerializeField]
        private bool _gamepadsArePlayers;

        public bool GamepadsArePlayers => _gamepadsArePlayers;

        [Space(10)]

#region Floating Text
        [Header("Floating text")]

        [SerializeField]
        [CanBeNull]
        private FloatingText _floatingTextPrefab;

        [CanBeNull]
        public FloatingText FloatingTextPrefab => _floatingTextPrefab;

        [SerializeField]
        private int _floatingTextPoolSize = 10;

        public int FloatingTextPoolSize => _floatingTextPoolSize;
#endregion
    }
}
