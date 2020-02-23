using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.Game.NPCs
{
    [Serializable]
    public class SpawnWave
    {
        private readonly SpawnWaveData _spawnWaveData;

        [SerializeField]
        [ReadOnly]
        private /*readonly*/ List<SpawnGroup> _spawnGroups = new List<SpawnGroup>();

        private TimedSpawnWaveData TimedSpawnWaveData => (TimedSpawnWaveData)_spawnWaveData;

        private readonly WaveSpawner _owner;

        [SerializeField]
        [ReadOnly]
        private int _spawnedCount;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _waveStartEffectTrigger;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _waveEndEffectTrigger;

        [SerializeReference]
        [ReadOnly]
        private ITimer _waveTimer;

        public SpawnWave(SpawnWaveData spawnWaveData, WaveSpawner owner)
        {
            _spawnWaveData = spawnWaveData;
            _owner = owner;

            foreach(SpawnGroupData spawnGroup in _spawnWaveData.SpawnGroups) {
                _spawnGroups.Add(new SpawnGroup(spawnGroup, _owner, this));
            }
        }

        public void Initialize()
        {
            if(null != _spawnWaveData.WaveStartEffectTriggerPrefab) {
                _waveStartEffectTrigger = _owner.AddWaveEffectTrigger(_spawnWaveData.WaveStartEffectTriggerPrefab);
            }

            if(null != _spawnWaveData.WaveEndEffectTriggerPrefab) {
                _waveStartEffectTrigger = _owner.AddWaveEffectTrigger(_spawnWaveData.WaveEndEffectTriggerPrefab);
            }

            if(_spawnWaveData is TimedSpawnWaveData) {
                _waveTimer = TimeManager.Instance.AddTimer();
                _waveTimer.TimesUpEvent += WaveTimerTimesUpEventHandler;
            }

            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Initialize();
            }
        }

        public void Shutdown()
        {
            if(null != _waveEndEffectTrigger) {
                _owner.RemoveWaveEffectTrigger(_waveEndEffectTrigger);
                _waveEndEffectTrigger = null;
            }

            if(null != _waveStartEffectTrigger) {
                _owner.RemoveWaveEffectTrigger(_waveStartEffectTrigger);
                _waveStartEffectTrigger = null;
            }

            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Shutdown();
            }

            if(TimeManager.HasInstance && null != _waveTimer) {
                TimeManager.Instance.RemoveTimer(_waveTimer);
            }
            _waveTimer = null;
        }

        public void Start()
        {
            if(null != _waveStartEffectTrigger) {
                _waveStartEffectTrigger.Trigger();
            }

            if(null != _waveTimer) {
                _waveTimer.Start(TimedSpawnWaveData.Duration);
            }

            _spawnedCount = 0;

            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Start();
            }
        }

        public void Stop()
        {
            if(null != _waveEndEffectTrigger) {
                _waveEndEffectTrigger.Trigger();
            }

            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Stop();
            }

            _spawnedCount = 0;

            if(null != _waveTimer) {
                _waveTimer.Stop();
            }
        }

#region Events
        public void OnWaveSpawned(int count)
        {
            _spawnedCount += count;
        }

        public void OnWaveSpawnMemberDone()
        {
            _spawnedCount--;

            if(_spawnedCount <= 0) {
                _owner.Advance();
            }
        }
#endregion

#region Event Handlers
        private void WaveTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            _owner.Advance();
        }
#endregion
    }
}
