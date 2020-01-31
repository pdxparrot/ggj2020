using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.KungFuCircle
{
    public class KungFuGrid : MonoBehaviour
    {
        // TODO: move this stuff to a data object
        [SerializeField]
        private int _maxGridSlots = 4;

        [SerializeField]
        private int _gridCapacity = 10;

        [SerializeField]
        private int _attackCapacity = 10;

        [SerializeField]
        private float _attackSlotDistance = 0.5f;

        [SerializeField]
        private Actor _owner;

        public Actor Owner => _owner;

        [SerializeField]
        [ReadOnly]
        private int[] _slotsTaken;

        [SerializeField]
        [ReadOnly]
        private float[] _innerGridSlotsRadians;

        [SerializeField]
        [ReadOnly]
        private float _radiansBetweenSlots;

#region Unity Lifecycle
        private void Awake()
        {
            _slotsTaken = new int[_maxGridSlots];
            _innerGridSlotsRadians = new float[_maxGridSlots];

            // TODO: do this smarter
            _radiansBetweenSlots = (360.0f / _maxGridSlots) * Mathf.Deg2Rad;

            float currentRadians = _radiansBetweenSlots;
            for(int i = 0; i < _maxGridSlots; ++i) {
                _innerGridSlotsRadians[i] += currentRadians;
                currentRadians += _radiansBetweenSlots;
            }
        }
#endregion

        public Vector3 GetAttackSlotLocation(int i)
        {
            float angle = _innerGridSlotsRadians[i];
            return Owner.Movement.Position + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * _attackSlotDistance;
        }

        public void EmptyGridSlot(int i)
        {
            _gridCapacity += _slotsTaken[i];
            _slotsTaken[i] = 0;
        }

        public int GetAvailableGridSlot(int gridWeight)
        {
            if(_gridCapacity - gridWeight < 0) {
                return -1;
            }

            for(int i = 0; i < _slotsTaken.Length; i++) {
                if(_slotsTaken[i] <= 0) {
                    _gridCapacity -= gridWeight;
                    _slotsTaken[i] = gridWeight;
                    return i;
                }
            }
            return -1;
        }

        // TODO: make use of these (need to know when the attack completes first)

        public bool AllocateAttack(int attackWeight)
        {
            if(_attackCapacity - attackWeight < 0) {
                return false;
            }

            _attackCapacity -= attackWeight;
            return true;
        }

        public void ReleaseAttack(int attackWeight)
        {
            _attackCapacity += attackWeight;
        }
    }
}
