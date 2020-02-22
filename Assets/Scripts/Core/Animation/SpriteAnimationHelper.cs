using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.Animation
{
    public class SpriteAnimationHelper : MonoBehaviour
    {
        [SerializeField]
        private List<SpriteRenderer> _renderers;

        public void AddRenderer(SpriteRenderer renderer)
        {
            _renderers.Add(renderer);
        }

        public void RemoveRenderer(SpriteRenderer renderer)
        {
            _renderers.Remove(renderer);
        }

        public void SetFacing(Vector3 direction)
        {
            if(Mathf.Approximately(direction.x, 0.0f)) {
                return;
            }

            foreach(SpriteRenderer r in _renderers) {
                r.flipX = direction.x < 0;
            }
        }
    }
}
