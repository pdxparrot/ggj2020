using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Time
{
    public sealed class TimeManager : SingletonBehavior<TimeManager>
    {
        public const float MilliSecondsToSeconds = 0.001f;

        [Serializable]
        private sealed class Timer : ITimer
        {
#region Events
            public event EventHandler<EventArgs> StartEvent;
            public event EventHandler<EventArgs> StopEvent;
            public event EventHandler<EventArgs> TimesUpEvent;
#endregion

            [SerializeField]
            private float _secondsRemaining;

            public float SecondsRemaining
            {
                get => _secondsRemaining;
                set => _secondsRemaining = value;
            }

            [SerializeField]
            private bool _isRunning;

            public bool IsRunning => _isRunning;

            public void Start(float timerSeconds)
            {
                if(IsRunning) {
                    return;
                }

                _secondsRemaining = timerSeconds;
                _isRunning = true;

                StartEvent?.Invoke(this, EventArgs.Empty);
            }

            public void StartMillis(long timerMs)
            {
                Start(timerMs * MilliSecondsToSeconds);
            }

            public void Start(IntRangeConfig timerSeconds)
            {
                if(IsRunning) {
                    return;
                }

                _secondsRemaining = timerSeconds.GetRandomValue();
                _isRunning = true;

                StartEvent?.Invoke(this, EventArgs.Empty);
            }

            public void ReStart(float timerSeconds)
            {
                Stop();
                Start(timerSeconds);
            }

            public void ReStartMillis(long timerMs)
            {
                ReStart(timerMs * MilliSecondsToSeconds);
            }

            public void Stop()
            {
                if(!IsRunning) {
                    return;
                }
                _isRunning = false;

                StopEvent?.Invoke(this, EventArgs.Empty);
            }

            public void Continue()
            {
                if(IsRunning) {
                    return;
                }
                _isRunning = true;

                StartEvent?.Invoke(this, EventArgs.Empty);
            }

            public void AddTime(float seconds)
            {
                _secondsRemaining += seconds;
            }

            public void Update(float dt)
            {
                if(!IsRunning) {
                    return;
                }

                _secondsRemaining -= dt;
                if(SecondsRemaining > 0.0f) {
                    return;
                }
                _secondsRemaining = 0.0f;
                _isRunning = false;

                TimesUpEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        [Serializable]
        private sealed class Stopwatch : IStopwatch
        {
#region Events
            public event EventHandler StartEvent;
            public event EventHandler StopEvent;
            public event EventHandler ResetEvent;
#endregion

            [SerializeField]
            private float _stopwatchSeconds;

            public float StopwatchSeconds => _stopwatchSeconds;

            [SerializeField]
            private bool _isRunning;

            public bool IsRunning => _isRunning;

            public void Start()
            {
                if(IsRunning) {
                    return;
                }
                _isRunning = true;

                StartEvent?.Invoke(this, EventArgs.Empty);
            }

            public void Stop()
            {
                if(!IsRunning) {
                    return;
                }
                _isRunning = false;

                StopEvent?.Invoke(this, EventArgs.Empty);
            }

            public void ResetStopwatch()
            {
                _stopwatchSeconds = 0.0f;

                ResetEvent?.Invoke(this, EventArgs.Empty);
            }

            public void Update(float dt)
            {
                if(!IsRunning) {
                    return;
                }

                _stopwatchSeconds += dt;
            }
        }

        public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public static long SecondsToMilliseconds(float seconds)
        {
            return (long)(seconds * 1000.0f);
        }

        private enum UpdateType
        {
            Update,
            Coroutine
        }

        [SerializeField]
        private UpdateType _updateType = UpdateType.Coroutine;

        [SerializeField]
        [Tooltip("How often to update when running in coroutine mode")]
        private float _updateRateMs = 100.0f;

        private Coroutine _updateRoutine;

        [SerializeField]
        private float _offsetSeconds;

        [SerializeField]
        [ReadOnly]
        private double _currentUnixSeconds;

        // NOTE: only accurate to the last update boundary
        public double CurrentUnixSeconds => _currentUnixSeconds;

        [SerializeField]
        [ReadOnly]
        private long _currentUnixMs;

        // NOTE: only accurate to the last update boundary
        public long CurrentUnixMs => _currentUnixMs;

        private readonly HashSet<Timer> _timers = new HashSet<Timer>();
        private readonly Dictionary<string, Timer> _namedTimers = new Dictionary<string, Timer>();

        private readonly HashSet<Stopwatch> _stopwatches = new HashSet<Stopwatch>();
        private readonly Dictionary<string, Stopwatch> _namedStopwatches = new Dictionary<string, Stopwatch>();

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();

            if(UpdateType.Coroutine == _updateType) {
                _updateRoutine = StartCoroutine(UpdateRoutine());
            }
        }

        protected override void OnDestroy()
        {
            StopCoroutine(_updateRoutine);
            _updateRoutine = null;

            base.OnDestroy();
        }

        private void Update()
        {
            if(_updateType != UpdateType.Update) {
                return;
            }

            float dt = UnityEngine.Time.deltaTime;

            DoUpdate(dt);
        }
#endregion

        private IEnumerator UpdateRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(_updateRateMs * MilliSecondsToSeconds);

            float lastRun = UnityEngine.Time.time;
            while(true) {
                yield return wait;

                float now = UnityEngine.Time.time;
                DoUpdate(now - lastRun);
                lastRun = now;
            }
        }

        private void DoUpdate(float dt)
        {
            // TODO: is there a way we can do this *without* allocating?
            _currentUnixSeconds = DateTime.UtcNow.Subtract(Epoch).TotalSeconds + _offsetSeconds;
            _currentUnixMs = (long)DateTime.UtcNow.Subtract(Epoch).TotalMilliseconds + SecondsToMilliseconds(_offsetSeconds);

            if(PartyParrotManager.Instance.IsPaused) {
                return;
            }

            UpdateStopwatches(dt);

            UpdateTimers(dt);
        }

        // NOTE: this ignores the game being paused
        public void RunAfterDelay(float seconds, Action action)
        {
            StartCoroutine(RunAfterDelayRoutine(seconds, action));
        }

#region Timers
        public ITimer AddTimer()
        {
            Timer timer = new Timer();
            _timers.Add(timer);
            return timer;
        }

        public bool RemoveTimer(ITimer timer)
        {
            if(null == timer) {
                return false;
            }

            timer.Stop();
            return _timers.Remove(timer as Timer);
        }

        public ITimer GetNamedTimer(string timerName)
        {
            return _namedTimers.GetOrAdd(timerName);
        }

        public bool RemoveNamedTimer(string timerName)
        {
            Timer timer = _namedTimers.GetOrDefault(timerName);
            if(null == timer) {
                return false;
            }

            timer.Stop();
            return _namedTimers.Remove(timerName);
        }

        private void UpdateTimers(float dt)
        {
            foreach(var timer in _timers) {
                timer.Update(dt);
            }

            foreach(var kvp in _namedTimers) {
                kvp.Value.Update(dt);
            }
        }
#endregion

#region Stopwatches
        public IStopwatch AddStopwatch()
        {
            Stopwatch stopwatch = new Stopwatch();
            _stopwatches.Add(stopwatch);
            return stopwatch;
        }

        public bool RemoveStopwatch(IStopwatch stopwatch)
        {
            if(null == stopwatch) {
                return false;
            }

            stopwatch.Stop();
            return _stopwatches.Remove(stopwatch as Stopwatch);
        }

        public IStopwatch GetNamedStopwatch(string stopwatchName)
        {
            return _namedStopwatches.GetOrAdd(stopwatchName);
        }

        public bool RemoveNamedStopwatch(string stopwatchName)
        {
            Stopwatch stopwatch = _namedStopwatches.GetOrDefault(stopwatchName);
            if(null == stopwatch) {
                return false;
            }

            stopwatch.Stop();
            return _namedStopwatches.Remove(stopwatchName);
        }

        private void UpdateStopwatches(float dt)
        {
            foreach(var stopwatch in _stopwatches) {
                stopwatch.Update(dt);
            }

            foreach(var kvp in _namedStopwatches) {
                kvp.Value.Update(dt);
            }
        }
#endregion

        private IEnumerator RunAfterDelayRoutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.TimeManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.Label($"Current Unix Seconds: {CurrentUnixSeconds}");
                GUILayout.Label($"Current Unix Milliseconds: {CurrentUnixMs}");

                // TODO: print timers

                // TODO: print stopwatches
            };
        }
    }
}
