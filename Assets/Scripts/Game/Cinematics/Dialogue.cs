using UnityEngine;

namespace pdxpartyparrot.Game.Cinematics
{
    [RequireComponent(typeof(RectTransform))]
    public class Dialogue : MonoBehaviour
    {
        [SerializeField]
        private string _id;

        public string Id => _id;
    }
}
