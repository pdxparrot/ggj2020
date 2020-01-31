using UnityEngine;
using UnityEngine.EventSystems;

namespace pdxpartyparrot.Core.UI
{
    public sealed class PhysicsRaycastSelector : MonoBehaviour
    {
        [SerializeField]
        private PhysicsRaycaster _physicsRaycaster;

#if ENABLE_GVR
        [SerializeField]
        private GvrPointerPhysicsRaycaster _gvrPhysicsRaycaster;
#endif


#region Unity Lifecycle
        private void Awake()
        {
#if ENABLE_VR
            _physicsRaycaster.enabled = !PartyParrotManager.Instance.EnableVR;
#endif

#if ENABLE_GVR
            _physicsRaycaster.enabled = !PartyParrotManager.Instance.EnableGoogleVR;
            _gvrPhysicsRaycaster.enabled = GameManager.Instance.EnableGoogleVR;
#endif
        }
#endregion
    }
}
