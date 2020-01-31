using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Time;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    [RequireComponent(typeof(PooledObject))]
    public class FloatingText : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1.0f;

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        [SerializeField]
        private float _lifeSpanSeconds = 5.0f;

        public float LifeSpanSeconds
        {
            get => _lifeSpanSeconds;
            set => _lifeSpanSeconds = value;
        }

        [SerializeField]
        private TextMeshPro _text;

        public TextMeshPro Text => _text;

        private Transform _transform;

        private PooledObject _pooledObject;

#region Unity Lifecycle
        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _pooledObject = GetComponent<PooledObject>();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            Float(dt);
        }
#endregion

        public void Show(Vector3 position)
        {
            _transform.position = position;
            TimeManager.Instance.RunAfterDelay(LifeSpanSeconds, () => {
                _pooledObject.Recycle();
            });
        }

        private void Float(float dt)
        {
            Vector3 position = _transform.position;
            position.y += Speed * dt;
            _transform.position = position;
        }
    }
}
