#define USE_LOG_MESSAGE_BUFFER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Core.DebugMenu
{
    // https://blogs.unity3d.com/2015/12/22/going-deep-with-imgui-and-editor-customization/
    public sealed class DebugMenuManager : SingletonBehavior<DebugMenuManager>
    {
#if !USE_LOG_MESSAGE_BUFFER
        private struct LogMessage
        {
            public string message;
            public LogType type;
        };
#endif

        [SerializeField]
        private Key _enableKey = Key.Backquote;

        [SerializeField]
        private bool _enabled;

        public bool Enabled => _enabled;

        [SerializeField]
        private Key _debugGUIEnableKey = Key.F11;

        [SerializeField]
        private bool _debugGUIEnabled = true;

        private DebugWindow _window;

        [SerializeField]
        [ReadOnly]
        private Vector2 _windowScrollPos;

        [SerializeField]
        [ReadOnly]
        private bool _showRendering;

        [SerializeField]
        [ReadOnly]
        private bool _showMemory;

        private readonly List<DebugMenuNode> _nodes = new List<DebugMenuNode>();

        [CanBeNull]
        private DebugMenuNode _currentNode;

        [SerializeField]
        [Range(0, 1000)]
        private int _fpsAccumulatorSize = 100;

        [SerializeField]
        private bool _frameStepping;

        private float _lastFrameTime;
        private float _maxFrameTime;
        private float _minFrameTime;

        private readonly Queue<float> _fpsAccumulator = new Queue<float>();

        private float AverageFPS => _fpsAccumulator.Count < 1 ? 0 : _fpsAccumulator.Average();

#if USE_LOG_MESSAGE_BUFFER
        private readonly StringBuilder _logMessageBuffer = new StringBuilder();
#else
        private readonly Queue<LogMessage> _logMessages = new Queue<LogMessage>();

        [SerializeField]
        private int _maxLogMessages = 1000;
#endif

#region Unity Lifecycle
        private void Awake()
        {
            ResetFrameStats();

#if !UNITY_EDITOR
            _showRendering = true;
            _showMemory = true;
#endif

            _window = new DebugWindow(new Rect(10, 10, 800, 600), RenderWindowContents)
            {
                Title = () => {
                    string title = "Debug Menu";

                    if(null != _currentNode) {
                        if(null != _currentNode.Parent) {
                            title = $"{_currentNode.Parent.Title()}";
                        }
                        title += $" => {_currentNode.Title()}";
                    }

                    return title;
                }
            };

            Application.logMessageReceived += OnLogMessageReceived;

            InitLogMessageDebugNode();
        }

        protected override void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;

            base.OnDestroy();
        }

        private void Update()
        {
            Profiler.BeginSample("DebugMenuManager.Update");
            try {
                if(_enabled) {
                    _window.Update();
                }
            } finally {
                Profiler.EndSample();
            }

            UpdateFrameStats(UnityEngine.Time.unscaledDeltaTime);

#if UNITY_EDITOR
            if(_frameStepping) {
                Debug.Break();
            }
#endif
        }

        private void FixedUpdate()
        {
            // TODO: this is dependent on the inputsystem being set to run
            // in fixed update. would be better if we could detect that somehow

            if(Keyboard.current[_enableKey].wasPressedThisFrame) {
                _enabled = !_enabled;
            }

            if(Keyboard.current[_debugGUIEnableKey].wasPressedThisFrame) {
                _debugGUIEnabled = !_debugGUIEnabled;
            }
        }

        private void OnGUI()
        {
            Profiler.BeginSample("DebugMenuManager.OnGUI");
            try {
                if(_enabled) {
                    _window.Render();
                }

#if UNITY_EDITOR
                RenderDebugUI();
#endif
            } finally {
                Profiler.EndSample();
            }
        }
#endregion

        public DebugMenuNode AddNode(Func<string> title)
        {
            DebugMenuNode node = new DebugMenuNode(title);
            _nodes.Add(node);
            return node;
        }

        public void RemoveNode(DebugMenuNode node)
        {
            if(null == node) {
                return;
            }

            _nodes.Remove(node);

            if(_currentNode == node) {
                SetCurrentNode(null);
            }
        }

        public void SetCurrentNode(DebugMenuNode node)
        {
            _currentNode = node;
            _windowScrollPos = Vector2.zero;
        }

        public void ResetFrameStats()
        {
            _lastFrameTime = 0;
            _minFrameTime = float.MaxValue;
            _maxFrameTime = float.MinValue;

            _fpsAccumulator.Clear();
        }

        private void UpdateFrameStats(float dt)
        {
            _lastFrameTime = dt;

            if(_lastFrameTime < _minFrameTime) {
                _minFrameTime = _lastFrameTime;
            }

            if(_lastFrameTime > _maxFrameTime) {
                _maxFrameTime = _lastFrameTime;
            }

            _fpsAccumulator.Enqueue(1.0f / _lastFrameTime);
            if(_fpsAccumulator.Count > _fpsAccumulatorSize) {
                _fpsAccumulator.Dequeue();
            }
        }

        private void RenderWindowContents()
        {
            if(null == _currentNode) {
                GUILayout.Label($"Unity version: {Application.unityVersion}");
                GUILayout.Label($"Random seed: {PartyParrotManager.Instance.RandomSeed}");

                _showRendering = GUIUtils.Foldout(_showRendering, "Rendering");
                if(_showRendering) {
                    GUILayout.BeginVertical(GUI.skin.box);
                        GUILayout.Label($"Frame Time: {(int)(_lastFrameTime * 1000.0f)} ms");
                        GUILayout.Label($"Min Frame Time: {(int)(_minFrameTime * 1000.0f)} ms");
                        GUILayout.Label($"Max Frame Time: {(int)(_maxFrameTime * 1000.0f)} ms");
                        GUILayout.Label($"Average FPS: {(int)AverageFPS}");
                    GUILayout.EndVertical();
                }

                _showMemory = GUIUtils.Foldout(_showMemory, "Memory");
                if(_showMemory) {
                    GUILayout.BeginVertical(GUI.skin.box);
                        GUILayout.Label($"Allocated: {Profiler.GetTotalAllocatedMemoryLong() / 1048576.0f:0.00}MB");
                        GUILayout.Label($"Reserved: {Profiler.GetTotalReservedMemoryLong() / 1048576.0f:0.00}MB");
                        GUILayout.Label($"Unused: {Profiler.GetTotalUnusedReservedMemoryLong() / 1048576.0f:0.00}MB");
                        GUILayout.Label($"Mono Heap: {Profiler.GetMonoHeapSizeLong() / 1048576.0f:0.00}MB");
                        GUILayout.Label($"Mono Used: {Profiler.GetMonoUsedSizeLong() / 1048576.0f:0.00}MB");
                        GUILayout.Label($"Temp Allocator Size: {Profiler.GetTempAllocatorSize() / 1048576.0f:0.00}MB");
#if UNITY_EDITOR
                        GUILayout.Label($"GPU Allocated: {Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576.0f:0.00}MB");
#endif
                    GUILayout.EndVertical();
                }

                _windowScrollPos = GUILayout.BeginScrollView(_windowScrollPos);
                    foreach(DebugMenuNode node in _nodes) {
                        node.RenderNode();
                    }
                GUILayout.EndScrollView();

#if UNITY_EDITOR
                GUILayout.BeginHorizontal();
                    if(GUIUtils.LayoutButton("Break")) {
                        Debug.Break();
                    }

                    _frameStepping = GUILayout.Toggle(_frameStepping, "Enable Frame Stepping (Manually disable in the Hierarchy)");
                    // TODO: should we force select the manager for the user?
                GUILayout.EndHorizontal();
#endif

                GUILayout.BeginHorizontal();
                    // TODO: reloading doesn't work right (it has to be in a coroutine or something right?)
                    /*if(GUIUtils.LayoutButton("Reload")) {
                        Scenes.SceneManager.Instance.ReloadMainScene();
                    }*/

                    if(GUIUtils.LayoutButton("Quit")) {
                        UnityUtil.Quit();
                    }

                    if(GUIUtils.LayoutButton("Reset PlayerPrefs")) {
                        PlayerPrefs.DeleteAll();
                    }
                GUILayout.EndHorizontal();
            } else {
                _windowScrollPos = GUILayout.BeginScrollView(_windowScrollPos);
                    _currentNode.RenderContents();
                GUILayout.EndScrollView();

                if(GUIUtils.LayoutButton("Back")) {
                    SetCurrentNode(_currentNode.Parent);
                }
            }
        }

        private void InitLogMessageDebugNode()
        {
            DebugMenuNode debugMenuNode = AddNode(() => "Logs");
            debugMenuNode.RenderContentsAction = () => {
#if USE_LOG_MESSAGE_BUFFER
                GUIStyle style = GUI.skin.textArea;
                style.richText = true;
                GUILayout.TextArea(_logMessageBuffer.ToString(), style);
#else
                foreach(LogMessage message in _logMessages) {
                    switch(message.type)
                    {
                    case LogType.Assert:
                        GUI.color = Color.green;
                        break;
                    case LogType.Warning:
                        GUI.color = Color.yellow;
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.color = Color.red;
                        break;
                    default:
                        GUI.color = Color.white;
                        break;
                    }
                    GUILayout.Label($"[{message.type}]: {message.message}");
                }
#endif
            };
        }

#if UNITY_EDITOR
        private void RenderDebugUI()
        {
            if(!_debugGUIEnabled) {
                return;
            }

            GUI.color = Color.white;

            GUILayout.BeginVertical();
                GUILayout.Label($"{_debugGUIEnableKey} to hide");
                GUILayout.Label($"{_enableKey} for debug menu");
                GUILayout.Label($"Average FPS: {(int)AverageFPS}");
                GUILayout.Label($"Allocated: {Profiler.GetTotalAllocatedMemoryLong() / 1048576.0f:0.00}MB / {Profiler.GetTotalReservedMemoryLong() / 1048576.0f:0.00}MB");
                GUILayout.Label($"Mono Used: {Profiler.GetMonoUsedSizeLong() / 1048576.0f:0.00}MB / {Profiler.GetMonoHeapSizeLong() / 1048576.0f:0.00}MB");
            GUILayout.EndVertical();
        }
#endif

#region Event Handlers
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
#if USE_LOG_MESSAGE_BUFFER
            string color;
            switch(type)
            {
            case LogType.Assert:
                color = "green";
                break;
            case LogType.Warning:
                color = "yellow";
                break;
            case LogType.Error:
            case LogType.Exception:
                color = "red";
                break;
            default:
                color = "white";
                break;
            }
            _logMessageBuffer.AppendLine($"<color={color}>[{type}]: {logString}</color>");
#else
            _logMessages.Enqueue(new LogMessage{ message = logString, type = type });
            if(_logMessages.Count > _maxLogMessages) {
                _logMessages.Dequeue();
            }
#endif
        }
#endregion
    }
}
