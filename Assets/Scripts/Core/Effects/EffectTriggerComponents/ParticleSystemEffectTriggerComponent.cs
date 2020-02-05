using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class ParticleSystemEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private ParticleSystem _vfx;

        [SerializeField]
        private bool _loop;

        [SerializeField]
        private bool _waitForComplete = true;

        [SerializeField]
        private bool _clearOnStop;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => !_vfx.isPlaying;

        public override void Initialize(EffectTrigger owner)
        {
            base.Initialize(owner);

            ParticleSystem.MainModule main = _vfx.main;
            main.loop = false;

            Assert.IsFalse(main.playOnAwake, $"ParticleSystem '{_vfx.name}' should not have playOnAwake set!");
        }

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableVFX) {
                ParticleSystem.MainModule main = _vfx.main;
                main.loop = _loop;

                _vfx.Play();
            } else {
                // TODO: set a timer or something to timeout when we'd normally be done
            }
        }

        public override void OnStop()
        {
            ParticleSystem.MainModule main = _vfx.main;
            main.loop = false;

            _vfx.Stop(true, _clearOnStop ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
            _vfx.time = 0.0f;
        }

        public override  void OnReset()
        {
            _vfx.Clear(true);
            _vfx.Simulate(0.0f, true, true);
        }
    }
}
