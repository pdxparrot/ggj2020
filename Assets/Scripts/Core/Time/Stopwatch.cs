using System;

namespace pdxpartyparrot.Core.Time
{
    public interface IStopwatch
    {
#region Events
        event EventHandler StartEvent;
        event EventHandler StopEvent;
        event EventHandler ResetEvent;
#endregion

        float StopwatchSeconds { get; }

        bool IsRunning { get; }

        void Start();

        void Stop();

        void ResetStopwatch();
    }
}
