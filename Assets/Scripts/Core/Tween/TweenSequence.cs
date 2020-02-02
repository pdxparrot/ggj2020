#if USE_DOTWEEN
using System;

using DG.Tweening;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public class TweenSequence : MonoBehaviour
    {
        [SerializeField]
        private bool _playOnAwake = false;

        [SerializeField]
        private bool _resetOnEnable = false;

#region Looping
        [SerializeField]
        private int _loops = 0;

        [SerializeField]
        LoopType _loopType = LoopType.Restart;
#endregion

#region Delay
        [SerializeField]
        private float _firstRunDelay = 0.0f;

        [SerializeField]
        private float _delay = 0.0f;
#endregion

        [SerializeField]
        [ReadOnly]
        private bool _firstRun = true;

        [SerializeField]
        [ReorderableList]
        private TweenRunner.ReorderableList _tweens = new TweenRunner.ReorderableList();

        [CanBeNull]
        private Sequence _sequence;

        public bool IsActive => null != _sequence && _sequence.IsActive();

        public bool IsRunning => IsActive && _sequence.IsPlaying();

#region Unity Lifecycle
        private void Awake()
        {
            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;

            foreach(TweenRunner runner in _tweens.Items) {
                // cleanup the runner start states so they don't act outside our control
                // TODO: this doesn't work :( bleh...
                runner.PlayOnAwake = false;
                runner.ResetOnEnable = false;
                runner.FirstRun = false;

                if(runner.IsInfiniteLoop) {
                    runner.Loops = 0;
                }
            }

            if(_playOnAwake) {
                Play();
            }
        }

        private void OnDestroy()
        {
            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }

        protected virtual void OnEnable()
        {
            if(_resetOnEnable) {
                DoReset();
                Play();
            }
        }
#endregion

        private void DoReset()
        {
            Kill();

            foreach(TweenRunner runner in _tweens.Items) {
                runner.DoReset();
            }
        }

        public void Play()
        {
            _sequence = DOTween.Sequence()
                .SetDelay(_firstRun ? (_firstRunDelay + _delay) : _delay)
                .SetLoops(_loops, _loopType);

            foreach(TweenRunner runner in _tweens.Items) {
                _sequence.Append(runner.Play());
            }

            _sequence.Play();

            _firstRun = false;
        }

        public void Pause()
        {
            if(IsRunning) {
                _sequence?.Pause();
            }
        }

        public void TogglePause()
        {
            if(IsActive) {
                _sequence?.TogglePause();
            }
        }

        public void Complete()
        {
            if(IsActive) {
                _sequence?.Complete();
            }
        }

        public void Complete(bool withCallbacks)
        {
            if(IsActive) {
                _sequence?.Complete(withCallbacks);
            }
        }

        public void Kill()
        {
            _sequence?.Kill();
        }

#region Events
        private void PauseEventHandler(object sender, EventArgs args)
        {
            TogglePause();
        }
#endregion
    }
}
#endif
