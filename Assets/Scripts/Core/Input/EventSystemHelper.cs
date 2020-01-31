using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace pdxpartyparrot.Core.Input
{
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public sealed class EventSystemHelper : MonoBehaviour
    {
        public EventSystem EventSystem { get; private set; }

        public InputSystemUIInputModule UIModule { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            EventSystem = GetComponent<EventSystem>();
            UIModule = GetComponent<InputSystemUIInputModule>();
        }
#endregion
    }
}
