﻿using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Core.UI
{
    public sealed class DebugWindow
    {
// TODO: this should support a lot more window-type options
// like "allow resize" and "allow move" and all that usual stuff

        private const int WindowHandleSize = 25;

        private static int _windowIdGenerator = 0;

        private static int NextId => ++_windowIdGenerator;

        public int Id { get; } = NextId;

        private Rect _rect;

#region Resizing
        private bool _isResizing;

        private Rect _resizeOriginalRect;

        private Vector2 _resizeStartPosition;
#endregion

        public Func<string> Title { get; set; } = () => "Debug Window";

        private readonly Action _renderContents;

        public DebugWindow(Rect rect, Action renderContents)
        {
            _rect = rect;
            _renderContents = renderContents;
        }

        public void Update()
        {
            Profiler.BeginSample("DebugWindow.Update");
            try {
                Resize();
            } finally {
                Profiler.EndSample();
            }
        }

        public void Render()
        {
            Profiler.BeginSample("DebugWindow.Render");
            try {
                _rect = GUILayout.Window(Id, _rect, id => {
                    _renderContents();

                    // TODO: we should limit dragging only to when it's done from the title bar
                    if(!_isResizing) {
                        GUI.DragWindow();
                    }
                }, Title());
            } finally {
                Profiler.EndSample();
            }
        }

        private void Resize()
        {
            // https://forum.unity.com/threads/is-there-a-resize-equivalent-to-gui-dragwindow.10144/
            var mousePosition = Mouse.current.position.ReadValue();
            mousePosition.y = Screen.height - mousePosition.y;

            Rect windowHandle = new Rect(_rect.x + _rect.width - WindowHandleSize, _rect.y + _rect.height - WindowHandleSize, WindowHandleSize, WindowHandleSize);

            if(Mouse.current.leftButton.wasPressedThisFrame && windowHandle.Contains(mousePosition)) {
                _isResizing = true;
                _resizeStartPosition = mousePosition;
                _resizeOriginalRect = _rect;
            }

            if(_isResizing) {
                if(Mouse.current.leftButton.isPressed) {
                    _rect.width = Mathf.Clamp(_resizeOriginalRect.width + (mousePosition.x - _resizeStartPosition.x), 0, Screen.width);
                    _rect.height = Mathf.Clamp(_resizeOriginalRect.height + (mousePosition.y - _resizeStartPosition.y), 0, Screen.height);
                }

                if(Mouse.current.leftButton.wasPressedThisFrame) {
                    _isResizing = false;
                }
            }
        }
    }
}
