using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.Game.NPCs
{
    [Serializable]
    internal class SpawnGroup
    {
        private readonly SpawnGroupData _spawnGroupData;

        [SerializeReference]
        [ReadOnly]
        private ITimer _spawnDelayTimer;

        private string PoolTag => $"spawnGroup_{_spawnGroupData.Tag}";

        private GameObject _poolContainer;

        private readonly  WaveSpawner _owner;

        private readonly SpawnWave _wave;

        public SpawnGroup(SpawnGroupData spawnGroupData, WaveSpawner owner, SpawnWave wave)
        {
            _spawnGroupData = spawnGroupData;
            _owner = owner;
            _wave = wave;
        }

        public void Initialize()
        {
            PooledObject pooledObject = _spawnGroupData.ActorPrefab.GetComponent<PooledObject>();
            if(null != pooledObject) {
                _poolContainer = new GameObject(PoolTag);
                _poolContainer.transform.SetParent(_owner.transform);

                int count = Mathf.Max(_spawnGroupData.PoolSize, 1);
                if(ObjectPoolManager.Instance.HasPool(PoolTag)) {
                    ObjectPoolManager.Instance.EnsurePoolSize(PoolTag, count);
                } else {
                    ObjectPoolManager.Instance.InitializePoolAsync(PoolTag, pooledObject, count);
                }
            }

            _spawnDelayTimer = TimeManager.Instance.AddTimer();
            _spawnDelayTimer.TimesUpEvent += SpawnTimerTimesUpEventHandler;
        }

        public void Shutdown()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_spawnDelayTimer);
            }
            _spawnDelayTimer = null;

            if(null != _poolContainer) {
                if(ObjectPoolManager.HasInstance) {
                    ObjectPoolManager.Instance.DestroyPool(PoolTag);
                }

                UnityEngine.Object.Destroy(_poolContainer);
                _poolContainer = null;
            }
        }

        public void Start()
        {
            _spawnDelayTimer.Start(_spawnGroupData.Delay);
        }

        public void Stop()
        {
            _spawnDelayTimer?.Stop();
        }

        private void Spawn()
        {
            int amount = _spawnGroupData.Count.GetRandomValue();

            Debug.Log($"Spawning {amount} NPCs...");

            int spawned = 0;
            for(int i=0; i<amount; ++i) {
                SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(_spawnGroupData.Tag);
                if(null == spawnPoint) {
                    //Debug.LogWarning($"No spawnpoints for {_spawnGroupData.Tag}!");
                } else {
                    Actor actor = null;
                    if(null != _poolContainer) {
                        actor = ObjectPoolManager.Instance.GetPooledObject<Actor>(PoolTag, _poolContainer.transform);
                        if(null == actor) {
                            Debug.LogWarning($"Actor for pool {PoolTag} missing its PooledObject!");
                            continue;
                        }

                        if(!spawnPoint.Spawn(actor, Guid.NewGuid(), _spawnGroupData.NPCBehaviorData)) {
                            actor.GetComponent<PooledObject>().Recycle();
                            Debug.LogWarning($"Failed to spawn actor for {_spawnGroupData.Tag}");
                            continue;
                        }
                    } else {
                        actor = spawnPoint.SpawnNPCPrefab(_spawnGroupData.ActorPrefab, _spawnGroupData.NPCBehaviorData, _poolContainer.transform);
                        if(null == actor) {
                            continue;
                        }
                    }

                    spawned++;
                }

                if(!_spawnGroupData.Once) {
                    _spawnDelayTimer.Start(_spawnGroupData.Delay);
                }
            }

            _wave.OnWaveSpawned(spawned);
        }

#region Event Handlers
        private void SpawnTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            Spawn();
        }
#endregion
    }
}
