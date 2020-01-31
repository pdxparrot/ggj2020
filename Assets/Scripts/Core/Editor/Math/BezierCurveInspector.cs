using pdxpartyparrot.Core.Math;

using UnityEditor;
using UnityEngine;

// https://catlikecoding.com/unity/tutorials/curves-and-splines/

namespace pdxpartyparrot.Core.Editor.Math
{
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveInspector : UnityEditor.Editor
    {
        private const int LineSteps = 10;

        private const float DirectionScale = 0.5f;

#region Unity Lifecycle
        private void OnSceneGUI()
        {
            BezierCurve curve = (BezierCurve)target;
            Transform handle = curve.transform;
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handle.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0, curve, handle, handleRotation);
            Vector3 p1 = ShowPoint(1, curve, handle, handleRotation);
            Vector3 p2 = ShowPoint(2, curve, handle, handleRotation);
            Vector3 p3 = ShowPoint(3, curve, handle, handleRotation);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            ShowDirections(curve);
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2.0f);
        }
#endregion

        private void ShowDirections(BezierCurve curve)
        {
            Handles.color = Color.green;
            Vector3 point = curve.GetPoint(0.0f);
            Handles.DrawLine(point, point + curve.GetDirection(0.0f) * DirectionScale);

            for(int i=1; i<=LineSteps; ++i) {
                point = curve.GetPoint(i / (float)LineSteps);
                Handles.DrawLine(point, point + curve.GetDirection(i / (float)LineSteps) * DirectionScale);
            }
        }

        private Vector3 ShowPoint(int index, BezierCurve curve, Transform handle, Quaternion handleRotation)
        {
            Vector3 point = handle.TransformPoint(curve.GetPoint(index));

            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.SetPoint(index, handle.InverseTransformPoint(point));
            }
            return point;
        }
    }
}
