using System;

using pdxpartyparrot.Core.Util;

namespace pdxpartyparrot.Core.Time
{
    public interface ITimer
    {
#region Events
        event EventHandler<EventArgs> StartEvent;
        event EventHandler<EventArgs> StopEvent;
        event EventHandler<EventArgs> TimesUpEvent;
#endregion

        float SecondsRemaining { get; set; }

        bool IsRunning { get; }

        void Start(float timerSeconds);

        void StartMillis(long timerMs);

        void Start(IntRangeConfig timerSeconds);

        void ReStart(float timerSeconds);

        void ReStartMillis(long timerMs);

        void Stop();

        void Continue();

        void AddTime(float seconds);
    }
}
