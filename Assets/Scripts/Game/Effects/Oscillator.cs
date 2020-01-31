using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Effects
{
    public class Oscillator : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _speed = new Vector3(1.0f, 1.0f, 1.0f);

        [SerializeField]
        private Vector3 _distance = new Vector3(1.0f, 1.0f, 1.0f);

        [SerializeField]
        private bool _localPosition = true;

        [SerializeField]
        private bool _randomizeOnEnable = true;

        [SerializeField]
        [ReadOnly]
        private Vector3 _angle;

        private Transform _transform;

#region Unity Lifecycle
        private void Awake()
        {
            // can't oscillate rigidbodies with this component
            Assert.IsNull(GetComponent<Rigidbody>());
            Assert.IsNull(GetComponent<Rigidbody2D>());

            _transform = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            if(!_randomizeOnEnable) {
                return;
            }

            _angle = new Vector3(PartyParrotManager.Instance.Random.NextSingle(0.0f, 2.0f * Mathf.PI),
                                 PartyParrotManager.Instance.Random.NextSingle(0.0f, 2.0f * Mathf.PI),
                                 PartyParrotManager.Instance.Random.NextSingle(0.0f, 2.0f * Mathf.PI));
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            Oscillate(dt);
        }
#endregion

        private void Oscillate(float dt)
        {
            _angle = new Vector3(MathUtil.WrapAngleRad(_angle.x + _speed.x * dt),
                                 MathUtil.WrapAngleRad(_angle.y + _speed.y * dt),
                                 MathUtil.WrapAngleRad(_angle.z + _speed.z * dt));

            Vector3 oscillate = new Vector3(Mathf.Sin(_angle.x) * _distance.x, 
                                            Mathf.Sin(_angle.y) * _distance.y,
                                            Mathf.Sin(_angle.z) * _distance.z);

            if(_localPosition) {
                _transform.localPosition = oscillate;
            } else {
                _transform.position = oscillate;
            }
        }
    }
}
