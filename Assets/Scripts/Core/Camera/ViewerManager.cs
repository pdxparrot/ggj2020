using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Camera
{
    public sealed class ViewerManager : SingletonBehavior<ViewerManager>
    {
        private class ViewerSet
        {
            public readonly HashSet<Viewer> Viewers = new HashSet<Viewer>();

            public readonly HashSet<Viewer> AssignedViewers = new HashSet<Viewer>();

            public readonly Queue<Viewer> UnassignedViewers = new Queue<Viewer>();

            public void AllocateViewers<T>(int count, T viewerPrefab, GameObject container) where T: Viewer
            {
                int actualCount = count - Viewers.Count;
                if(actualCount <= 0) {
                    return;
                }

                Debug.Log($"Allocating {actualCount} viewers of type {typeof(T)}...");

                for(int i=0; i<actualCount; ++i) {
                    Viewer viewer = Instantiate(viewerPrefab, container.transform);
                    viewer.Initialize(i);
                    viewer.gameObject.SetActive(false);

                    Viewers.Add(viewer);
                    UnassignedViewers.Enqueue(viewer);
                }
            }

            public void FreeViewers()
            {
                Debug.Log($"Freeing {Viewers.Count} viewers...");

                AssignedViewers.Clear();
                UnassignedViewers.Clear();

                foreach(Viewer viewer in Viewers) {
                    Destroy(viewer.gameObject);
                }

                Viewers.Clear();
            }

            public T AcquireViewer<T>() where T: Viewer
            {
                if(UnassignedViewers.Count < 1) {
                    Debug.LogWarning($"Attempt to acquire a viewer of type {typeof(T)} when there are none!");
                    return null;
                }

                Viewer viewer = UnassignedViewers.Dequeue();
                viewer.gameObject.SetActive(true);

                AssignedViewers.Add(viewer);

                if(ViewerManager.Instance.EnableDebug) {
                    Debug.Log($"Acquired viewer {viewer.name} (type: {typeof(T)}, assigned: {AssignedViewers.Count}, unassigned: {UnassignedViewers.Count})");
                }
                return viewer as T;
            }

            public void ReleaseViewer<T>(T viewer) where T: Viewer
            {
                if(!AssignedViewers.Contains(viewer)) {
                    // TODO: log a warning?
                    return;
                }

                if(ViewerManager.Instance.EnableDebug) {
                    Debug.Log($"Releasing viewer {viewer.name} (type: {typeof(T)}, assigned: {AssignedViewers.Count}, unassigned: {UnassignedViewers.Count})");
                }

                viewer.ResetViewer();

                viewer.gameObject.SetActive(false);

                AssignedViewers.Remove(viewer);
                UnassignedViewers.Enqueue(viewer);
            }

            public void ResetViewers<T>() where T: Viewer
            {
                Debug.Log($"Releasing all ({UnassignedViewers.Count}) {typeof(T)} viewers");

                // we loop through all of the viewers
                // because we can't loop over the assigned viewers
                foreach(Viewer viewer in Viewers) {
                    ReleaseViewer(viewer);

                    Transform viewerTransform = viewer.transform;
                    viewerTransform.position = Vector3.zero;
                    viewerTransform.rotation = Quaternion.identity;
                }
            }
        }

        [SerializeField]
        private float _viewportEpsilon = 0.005f;

        public float ViewportEpsilon => _viewportEpsilon;

        private readonly Dictionary<Type, ViewerSet> _viewers = new Dictionary<Type,ViewerSet>();

        private GameObject _viewerContainer;

#region Debug
        [SerializeField]
        private bool _enableDebug;

        public bool EnableDebug => _enableDebug;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _viewerContainer = new GameObject("Viewers");

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            Destroy(_viewerContainer);
            _viewerContainer = null;

            base.OnDestroy();
        }
#endregion

#region Allocate
        public void AllocateViewers<T>(int count, T viewerPrefab) where T: Viewer
        {
            Type viewerType = viewerPrefab.GetType();
            ViewerSet viewerSet = _viewers.GetOrAdd(viewerType);
            viewerSet.AllocateViewers(count, viewerPrefab, _viewerContainer);

            ResizeViewports();
        }

        public void FreeViewers<T>() where T: Viewer
        {
            Type viewerType = typeof(T);
            if(!_viewers.TryGetValue(viewerType, out var viewerSet)) {
                Debug.LogWarning($"Attempt to free unallocated viewers of type {viewerType}");
                return;
            }

            viewerSet.FreeViewers();
        }

        public void FreeAllViewers()
        {
            foreach(var kvp in _viewers) {
                kvp.Value.FreeViewers();
            }
        }
#endregion

#region Acquire
        [CanBeNull]
        public T AcquireViewer<T>() where T: Viewer
        {
            Type viewerType = typeof(T);
            if(!_viewers.TryGetValue(viewerType, out var viewerSet)) {
                Debug.LogWarning($"Attempt to acquire unallocated viewer of type {viewerType}");
                return null;
            }

            return viewerSet.AcquireViewer<T>();
        }

        public void ReleaseViewer<T>(T viewer) where T: Viewer
        {
            Type viewerType = viewer.GetType();
            if(!_viewers.TryGetValue(viewerType, out var viewerSet)) {
                Debug.LogWarning($"Attempt to release unallocated viewer of type {viewerType}");
                return;
            }

            viewerSet.ReleaseViewer(viewer);
        }

        public void ResetViewers<T>() where T: Viewer
        {
            Type viewerType = typeof(T);
            if(!_viewers.TryGetValue(viewerType, out var viewerSet)) {
                Debug.LogWarning($"Attempt to reset unallocated viewers of type {viewerType}");
                return;
            }

            viewerSet.ResetViewers<T>();

            ResizeViewports();
        }
#endregion

        public void ResizeViewports()
        {
            foreach(var kvp in _viewers) {
                ViewerSet viewerSet = kvp.Value;
                if(viewerSet.AssignedViewers.Count > 0) {
                    ResizeViewports(viewerSet.AssignedViewers);
                } else if(viewerSet.UnassignedViewers.Count > 0) {
                    ResizeViewports(viewerSet.UnassignedViewers);
                }
            }
        }

        private void ResizeViewports(IReadOnlyCollection<Viewer> viewers)
        {
            int gridCols = Mathf.CeilToInt(Mathf.Sqrt(viewers.Count));
            int gridRows = gridCols;

            // remove any extra full colums
            int extraCols = (gridCols * gridRows) - viewers.Count;
            gridCols -= extraCols / gridRows;

            float viewportWidth = (1.0f / gridCols);
            float viewportHeight = (1.0f / gridRows);

            Debug.Log($"Resizing {viewers.Count} viewports, Grid Size: {gridCols}x{gridRows} Viewport Size: {viewportWidth}x{viewportHeight}");

            for(int row=0; row<gridRows; ++row) {
                for(int col=0; col<gridCols; ++col) {
                    int viewerIdx = (row * gridCols) + col;
                    if(viewerIdx >= viewers.Count) {
                        break;
                    }
                    viewers.ElementAt(viewerIdx).SetViewport(col, (gridRows - 1) - row, viewportWidth, viewportHeight);
                }
            }
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.ViewerManager");
            debugMenuNode.RenderContentsAction = () => {
                _enableDebug = GUILayout.Toggle(_enableDebug, "Enable Debug");

                foreach(var kvp in _viewers) {
                    ViewerSet viewerSet = kvp.Value;
                    GUILayout.BeginVertical($"{kvp.Key} Viewers", GUI.skin.box);
                        GUILayout.Label($"Total Viewers: {viewerSet.Viewers.Count}");
                        GUILayout.Label($"Assigned Viewers: {viewerSet.AssignedViewers.Count}");
                        GUILayout.Label($"Unassigned Viewers: {viewerSet.UnassignedViewers.Count}");
                    GUILayout.EndVertical();
                }
            };
        }
    }
}
