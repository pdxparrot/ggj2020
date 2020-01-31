using pdxpartyparrot.Core.Splines;

using UnityEditor;
using UnityEngine;

// https://catlikecoding.com/unity/tutorials/curves-and-splines/

namespace pdxpartyparrot.Core.Editor.Splines
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : UnityEditor.Editor
    {
        private const int StepsPerCurve = 10;
        private const float DirectionScale = 0.5f;
        private const float HandleSize = 0.04f;
        private const float PickSize = 0.06f;

        private static readonly Color[] ModeColors =
        {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        private int _selectedIndex = -1;

#region Unity Lifecycle
        public override void OnInspectorGUI()
        {
            BezierSpline spline = (BezierSpline)target;

            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Toggle Loop");
                EditorUtility.SetDirty(spline);
                spline.Loop = loop;
            }

            if(_selectedIndex >= 0 && _selectedIndex < spline.ControlPointCount) {
                DrawSelectedPointInspector(spline);
            }

            if(GUILayout.Button("Add Curve")) {
                Undo.RecordObject(spline, "Add Curve");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }
        }

        private void OnSceneGUI()
        {
            BezierSpline spline = (BezierSpline)target;
            Transform handle = spline.transform;
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handle.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0, spline, handle, handleRotation);
            for(int i=1; i<spline.ControlPointCount; i += 3) {
                Vector3 p1 = ShowPoint(i   , spline, handle, handleRotation);
                Vector3 p2 = ShowPoint(i + 1, spline, handle, handleRotation);
                Vector3 p3 = ShowPoint(i + 2, spline, handle, handleRotation);
                
                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }

            ShowDirections(spline);
        }
#endregion

        private void DrawSelectedPointInspector(BezierSpline spline)
        {
            GUILayout.Label("Selected Point");

            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(_selectedIndex));
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(_selectedIndex, point);
            }

            EditorGUI.BeginChangeCheck();
            BezierSpline.Mode mode = (BezierSpline.Mode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(_selectedIndex));
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(_selectedIndex, mode);
                EditorUtility.SetDirty(spline);
            }
        }

        private void ShowDirections(BezierSpline spline)
        {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Handles.DrawLine(point, point + spline.GetDirection(0.0f) * DirectionScale);

            int steps = StepsPerCurve * spline.CurveCount;
            for(int i=1; i<=steps; ++i) {
                point = spline.GetPoint(i / (float)steps);
                Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * DirectionScale);
            }
        }

        private Vector3 ShowPoint(int index, BezierSpline spline, Transform handle, Quaternion handleRotation)
        {
            Vector3 point = handle.TransformPoint(spline.GetControlPoint(index));
            float size = HandleUtility.GetHandleSize(point);
            if(index == 0) {
                size *= 2.0f;
            }

            Handles.color = ModeColors[(int)spline.GetControlPointMode(index)];
            if(Handles.Button(point, handleRotation, size * HandleSize, size * PickSize, Handles.DotHandleCap)) {
                _selectedIndex = index;
                Repaint();
            }

            if(_selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if(EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(index, handle.InverseTransformPoint(point));
                }
            }

            return point;
        }
    }
}
