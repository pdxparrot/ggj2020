#if USE_SPINE
using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Animation;

using Spine;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SpineAnimationEffectTriggerComponent : EffectTriggerComponent
    {
        public class EventArgs : System.EventArgs
        {
            public TrackEntry TrackEntry { get; set; }
        }

#region Events
        public event EventHandler<EventArgs> StartEvent;
        public event EventHandler<EventArgs> CompleteEvent;
#endregion

        [SerializeField]
        private SpineAnimationHelper _spineAnimation;

        [SerializeField]
        private string _spineAnimationName = "default";

        public string SpineAnimationName
        {
            get => _spineAnimationName;
            set => _spineAnimationName = value;
        }

        [SerializeField]
        private int _spineAnimationTrack;

        public int SpineAnimationTrack
        {
            get => _spineAnimationTrack;
            set => _spineAnimationTrack = value;
        }

        [SerializeField]
        private bool _loop;

        public bool Loop
        {
            get => _loop;
            set => _loop = value;
        }

        [SerializeField]
        [Min(0.0f)]
        private float _trackStartTime;

        public float TrackStartTime
        {
            get => _trackStartTime;
            set => _trackStartTime = Mathf.Max(value, 0.0f);
        }

        [SerializeField]
        private bool _waitForComplete = true;

        // don't wait for complete if the animation should loop
        public override bool WaitForComplete => !Loop && _waitForComplete;

        // looping animations never "complete", so consider them always done
        public override bool IsDone => null == _trackEntry || _trackEntry.Loop || _trackEntry.IsComplete;

        [CanBeNull]
        private TrackEntry _trackEntry;

#region Unity Lifecycle
        private void Awake()
        {
            if(Loop) {
                Assert.IsFalse(_waitForComplete);
            }
        }
#endregion

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableAnimation) {
                if(EffectsManager.Instance.EnableDebug) {
                    Debug.Log($"Triggering Spine animation {_spineAnimationName} on track {_spineAnimationTrack} (loop={Loop}, start time={TrackStartTime})");
                }

                _trackEntry = _spineAnimation.SetAnimation(_spineAnimationTrack, _spineAnimationName, Loop);
                if(null != _trackEntry) {
                    _trackEntry.TrackTime = TrackStartTime;
                    _trackEntry.Complete += OnComplete;

                    StartEvent?.Invoke(this, new EventArgs{
                        TrackEntry = _trackEntry,
                    });
                }
            } else {
                // TODO: set a timer or something to timeout when we'd normally be done
            }
        }

        public override void OnStop()
        {
            // TODO: any way to force-stop the animation?

            Complete();
        }

        public override void OnReset()
        {
            _spineAnimation.ResetAnimation();
        }

        private void Complete()
        {
            if(null == _trackEntry) {
                return;
            }

            CompleteEvent?.Invoke(this, new EventArgs{
                TrackEntry = _trackEntry,
            });

            _trackEntry.Complete -= OnComplete;
            _trackEntry = null;
        }

#region Event Handlers
        private void OnComplete(TrackEntry entry)
        {
            if(entry != _trackEntry) {
                // is this even possible?
                return;
            }

            if(EffectsManager.Instance.EnableDebug) {
                Debug.Log($"Spine animation {_spineAnimationName} on track {_spineAnimationTrack} complete");
            }

            // for whatever reason, this is necessary to stop tracks breaking each other
            if(!entry.Loop) {
                _spineAnimation.SetEmptyAnimation(_spineAnimationTrack);
            }

            Complete();
        }
#endregion
    }
}
#endif
