using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class LogEffectTriggerComponent : EffectTriggerComponent
    {
        private enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        [SerializeField]
        private string _logMessage;

        [SerializeField]
        private LogLevel _level = LogLevel.Info;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            switch(_level)
            {
            case LogLevel.Info:
                Debug.Log(_logMessage);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(_logMessage);
                break;
            case LogLevel.Error:
                Debug.LogError(_logMessage);
                break;
            }
        }
    }
}
