using UnityEngine;

namespace pdxpartyparrot.Game.Cinematics
{
    public abstract class Cinematic : MonoBehaviour
    {
        [SerializeField]
        private string _id;

        public string Id => _id;

        public virtual void Play()
        {
        }

        public virtual void Stop()
        {
        }
    }
}
