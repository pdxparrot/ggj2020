using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    public sealed class FloatingTextQueue : MonoBehaviour
    {
        private struct FloatingTextEntry
        {
            public string poolName;

            public string text;

            public Color color;

            public Func<Vector3> position;
        }

        [SerializeField]
        private float _spawnRate = 0.1f;

        [SerializeField]
        private float _spawnOffset = 0.5f;

        [SerializeField]
        [ReadOnly]
        private float _nextSpawnOffset;

        private readonly Queue<FloatingTextEntry> _floatingText = new Queue<FloatingTextEntry>();

#region Unity Lifecycle
        private void Awake()
        {
            StartCoroutine(SpawnRoutine());
        }
#endregion

        public void QueueFloatingText(string text, Color color, Func<Vector3> position)
        {
            QueueFloatingText(GameStateManager.Instance.GameUIManager.DefaultFloatingTextPoolName, text, color, position);
        }

        public void QueueFloatingText(string poolName, string text, Color color, Func<Vector3> position)
        {
            _floatingText.Enqueue(new FloatingTextEntry
            {
                poolName = poolName,
                text = text,
                color = color,
                position = position
            });
        }

        private IEnumerator SpawnRoutine()
        {
            // start at an extremity
            _nextSpawnOffset = _spawnOffset;

            WaitForSeconds wait = new WaitForSeconds(_spawnRate);
            while(true) {
                yield return wait;

                if(_floatingText.Count < 1) {
                    continue;
                }

                FloatingTextEntry entry = _floatingText.Dequeue();

                FloatingText floatingText = GameStateManager.Instance.GameUIManager.InstantiateFloatingText(entry.poolName);
                if(null == floatingText) {
                    Debug.LogWarning($"Failed to get floating text from pool {entry.poolName}!");
                    continue;
                }

                // offset our starting x (TODO: offset z also ?)
                Vector3 position = entry.position();
                position.x += _nextSpawnOffset;
                _nextSpawnOffset = -_nextSpawnOffset;

                floatingText.Text.text = entry.text;
                floatingText.Text.color = entry.color;
                floatingText.Show(position);
            }
        }
    }
}
