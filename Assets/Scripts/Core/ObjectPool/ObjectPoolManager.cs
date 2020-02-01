#pragma warning disable 0618    // disable obsolete warning for now

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.ObjectPool
{
    public sealed class ObjectPoolManager : SingletonBehavior<ObjectPoolManager>
    {
        private sealed class ObjectPool
        {
            private int _initialTargetSize;

            public int TargetSize { get; set; }

            public int Size { get; private set; }

            public int Count => _pooledObjects.Count;

            public string Tag { get; }

            public bool IsNetwork { get; }

            public bool AllowExpand { get; set; } = true;

            public PooledObject Prefab { get; }

            public Coroutine Expander { get; set; }

            private readonly Queue<PooledObject> _pooledObjects;

            private GameObject _container;

            private Coroutine _expandRoutine;

            public ObjectPool(GameObject parent, string tag, PooledObject prefab, int size, bool isNetwork)
            {
                Tag = tag;
                Prefab = prefab;
                IsNetwork = isNetwork;

                _initialTargetSize = size;
                TargetSize = size;

                _container = new GameObject(Tag);
                _container.transform.SetParent(parent.transform);

                _pooledObjects = new Queue<PooledObject>(TargetSize);

                // start at our target size
                // (this is a performance hit for large pools)
                DoExpand(TargetSize);
            }

            public void Destroy()
            {
                Object.Destroy(_container);
                _container = null;
            }

            [CanBeNull]
            public PooledObject GetPooledObject(Transform parent=null, bool activate=true)
            {
                if(!_pooledObjects.Any()) {
                    if(!AllowExpand) {
                        return null;
                    }

                    // only grow if we've reached our target
                    if(Size >= TargetSize) {
                        Debug.LogWarning($"Growing object pool {Tag} by {_initialTargetSize}!");
                        TargetSize += _initialTargetSize;
                    }

                    // expand by 1 immediately so we have something to return
                    DoExpand(1);
                }

                // NOTE: reparent then activate to avoid hierarchy rebuild
                PooledObject pooledObject = _pooledObjects.Dequeue();
                pooledObject.transform.SetParent(parent);
                pooledObject.gameObject.SetActive(activate);

                if(IsNetwork) {
                    Network.NetworkManager.Instance.SpawnNetworkObject(pooledObject.GetComponent<NetworkBehaviour>());
                }

                return pooledObject;
            }

            public IEnumerator ExpandRoutine()
            {
                WaitForSeconds wait = new WaitForSeconds(Instance.ExpandCooldownSeconds);
                while(true) {
                    yield return wait;

                    if(Size >= TargetSize) {
                        continue;
                    }

                    int amount = Mathf.Min(TargetSize - Size, Instance.MaxExpandAmount);
                    Debug.Log($"Expanding object pool {Tag} by {amount}");
                    DoExpand(amount);
                }
            }

            private void DoExpand(int amount)
            {
                Assert.IsTrue(!IsNetwork || NetworkManager.Instance.IsServerActive());

                if(amount <= 0) {
                    return;
                }

                for(int i=0; i<amount; ++i) {
                    PooledObject pooledObject = Instantiate(Prefab);
                    pooledObject.Tag = Tag;
                    Recycle(pooledObject);
                }

                Size += amount;
            }

            public void EnsureSize(int size)
            {
                int amount = size - TargetSize;
                if(amount <= 0) {
                    return;
                }

                TargetSize += amount;
            }

            public void Recycle(PooledObject pooledObject)
            {
                if(IsNetwork) {
                    Network.NetworkManager.Instance.DeSpawnNetworkObject(pooledObject.GetComponent<NetworkBehaviour>());
                }

                // NOTE: de-activate and then reparent to avoid hierarchy rebuild
                pooledObject.gameObject.SetActive(false);
                pooledObject.transform.SetParent(_container.transform);

                _pooledObjects.Enqueue(pooledObject);
            }
        }

        [SerializeField]
        [Tooltip("The time between expand routine frames")]
        private float _expandCooldownMs = 100.0f;

        public float ExpandCooldownSeconds => _expandCooldownMs * TimeManager.MilliSecondsToSeconds;

        [SerializeField]
        [Tooltip("The number of pooled objects to expand by each frame of the expand routine")]
        private int _maxExpandAmount = 5;

        public int MaxExpandAmount => _maxExpandAmount;

        private readonly Dictionary<string, ObjectPool> _objectPools = new Dictionary<string, ObjectPool>();

        private GameObject _poolContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _poolContainer = new GameObject("Object Pools");

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            foreach(var kvp in _objectPools) {
                StopCoroutine(kvp.Value.Expander);
                kvp.Value.Expander = null;

                kvp.Value.Destroy();
            }
            _objectPools.Clear();

            Destroy(_poolContainer);
            _poolContainer = null;

            base.OnDestroy();
        }
#endregion

        public bool HasPool(string poolTag)
        {
            return _objectPools.ContainsKey(poolTag);
        }

        public void InitializePoolAsync(string poolTag, PooledObject prefab, int size, bool allowExpand=true)
        {
            Assert.IsNotNull(prefab);

            bool isNetwork = false;

            NetworkBehaviour networkBehaviour = prefab.GetComponent<NetworkBehaviour>();
            if(null != networkBehaviour) {
                isNetwork = true;

                Network.NetworkManager.Instance.RegisterNetworkPrefab(networkBehaviour);

                // network pools are server-only
                if(!NetworkManager.Instance.IsServerActive()) {
                    return;
                }
            }

            Debug.Log($"Initializing {(isNetwork ? "network" : "local")} object pool of size {size} for {poolTag} (allowExpand={allowExpand})");

            ObjectPool objectPool = _objectPools.GetOrDefault(poolTag);
            if(null != objectPool) {
                return;
            }

            objectPool = new ObjectPool(_poolContainer, poolTag, prefab, size, isNetwork)
            {
                AllowExpand = allowExpand
            };
            objectPool.Expander = StartCoroutine(objectPool.ExpandRoutine());

            _objectPools.Add(poolTag, objectPool);
        }

        public void DestroyPool(string poolTag)
        {
            ObjectPool objectPool = _objectPools.GetOrDefault(poolTag);
            if(null == objectPool) {
                return;
            }

            if(objectPool.IsNetwork) {
                Network.NetworkManager.Instance.UnregisterNetworkPrefab(objectPool.Prefab.GetComponent<NetworkBehaviour>());
            }

            _objectPools.Remove(poolTag);

            StopCoroutine(objectPool.Expander);
            objectPool.Expander = null;

            objectPool.Destroy();
        }

        public void ExpandPool(string poolTag, int amount)
        {
            ObjectPool pool = _objectPools.GetOrDefault(poolTag);
            if(null == pool) {
                Debug.LogWarning($"No pool for tag {poolTag}!");
                return;
            }
            pool.TargetSize += amount;
        }

        public void EnsurePoolSize(string poolTag, int size)
        {
            ObjectPool pool = _objectPools.GetOrDefault(poolTag);
            if(null == pool) {
                Debug.LogWarning($"No pool for tag {poolTag}!");
                return;
            }
            pool.EnsureSize(size);
        }

        [CanBeNull]
        public PooledObject GetPooledObject(string poolTag, Transform parent=null, bool activate=true)
        {
            ObjectPool pool = _objectPools.GetOrDefault(poolTag);
            if(null == pool) {
                Debug.LogWarning($"No pool for tag {poolTag}!");
                return null;
            }
            return pool.GetPooledObject(parent, activate);
        }

        [CanBeNull]
        public T GetPooledObject<T>(string poolTag, Transform parent=null, bool activate=true) where T: Component
        {
            PooledObject po = GetPooledObject(poolTag, parent, activate);
            if(null != po) {
                return po.GetComponent<T>();
            }
            return null;
        }

        public void Recycle(PooledObject pooledObject)
        {
            ObjectPool pool = _objectPools.GetOrDefault(pooledObject.Tag);
            if(null == pool) {
                Debug.LogWarning($"No pool for recycled object {pooledObject.name}, destroying...");
                Destroy(pooledObject.gameObject);
            } else {
                pool.Recycle(pooledObject);
            }
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.ObjectPoolManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Object Pools:", GUI.skin.box);
                    foreach(var kvp in _objectPools) {
                        GUILayout.BeginVertical(kvp.Key, GUI.skin.box);
                            GUILayout.Label($"Expandable: {kvp.Value.AllowExpand}");
                            GUILayout.Label($"Networked: {kvp.Value.IsNetwork}");
                            GUILayout.Label($"Size: {kvp.Value.Count} / {kvp.Value.Size} / {kvp.Value.TargetSize}");
                        GUILayout.EndVertical();
                    }
                GUILayout.EndVertical();
            };
        }
    }
}
