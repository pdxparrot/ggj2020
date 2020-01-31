using System.Collections.Generic;

using pdxpartyparrot.Core.Math;

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
            if(Mathf.Abs(direction.x) < MathUtil.Epsilon) {
                return;
            }

            foreach(SpriteRenderer r in _renderers) {
                r.flipX = direction.x < 0;
            }
        }
    }
}
