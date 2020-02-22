using UnityEngine;

namespace pdxpartyparrot.Core.Editor.Util
{
    public class Notes : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        [TextArea]
        private string _notes;
#endif
    }
}
