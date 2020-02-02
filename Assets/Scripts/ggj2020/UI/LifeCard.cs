using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    public sealed class LifeCard : MonoBehaviour
    {
        [SerializeField]
        private GameObject _success;

        [SerializeField]
        private GameObject _fail;

#region Unity Lifecycle
        private void Awake()
        {
            _success.SetActive(true);
            _fail.SetActive(false);
        }
#endregion

        public void Fail()
        {
            _success.SetActive(false);
            _fail.SetActive(true);
        }
    }
}
