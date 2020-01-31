using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIObject : MonoBehaviour
    {
        [SerializeField]
        private string _id;

        public string Id => _id;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            UIManager.Instance.RegisterUIObject(this);
        }

        protected virtual void OnDestroy()
        {
            if(UIManager.HasInstance) {
                UIManager.Instance.UnregisterUIObject(this);
            }
        }
#endregion
    }
}
