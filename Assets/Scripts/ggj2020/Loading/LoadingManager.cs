using pdxpartyparrot.Game.Loading;
using pdxpartyparrot.ggj2020.NPCs;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.UI;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Loading
{
    public sealed class LoadingManager : LoadingManager<LoadingManager>
    {
        [Space(10)]

#region Manager Prefabs
        [Header("Project Manager Prefabs")]

        [SerializeField]
        private GameManager _gameManagerPrefab;

        [SerializeField]
        private PlayerManager _playerManagerPrefab;

        [SerializeField]
        private NPCManager _npcManagerPrefab;

        [SerializeField]
        private GameUIManager _gameUiManagerPrefab;
#endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.CreateFromPrefab(_gameManagerPrefab, ManagersContainer);
            PlayerManager.CreateFromPrefab(_playerManagerPrefab, ManagersContainer);
            NPCManager.CreateFromPrefab(_npcManagerPrefab, ManagersContainer);
            GameUIManager.CreateFromPrefab(_gameUiManagerPrefab, ManagersContainer);
        }
    }
}
