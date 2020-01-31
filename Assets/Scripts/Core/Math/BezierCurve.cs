using UnityEngine;

// https://catlikecoding.com/unity/tutorials/curves-and-splines/

namespace pdxpartyparrot.Core.Math
{
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField]
        private Vector3[] _points;

        public int PointCount => _points.Length;

#if UNITY_EDITOR
        public Vector3 GetPoint(int index)
        {
            return _points[index];
        }

        public void SetPoint(int index, Vector3 point)
        {
            _points[index] = point;
        }
#endif
        
        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(Bezier.GetPoint(_points[0], _points[1], _points[2], _points[3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            Vector3 position = transform.position;
            return transform.TransformPoint(Bezier.GetFirstDerivative(_points[0], _points[1], _points[2], _points[3], t)) - position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public void ResetCurve()
        {
            _points = new[] {
                       Vector3.right,
                2.0f * Vector3.right,
                3.0f * Vector3.right,
                4.0f * Vector3.right
            };
        }
    }
}
