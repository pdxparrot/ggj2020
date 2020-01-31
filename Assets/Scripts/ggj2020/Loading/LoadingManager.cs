using pdxpartyparrot.Game.Loading;

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
#endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.CreateFromPrefab(_gameManagerPrefab, ManagersContainer);
        }
    }
}
