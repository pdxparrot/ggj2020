using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    [Serializable]
    public struct IntRangeConfig
    {
        [SerializeField]
        private int _min;

        public int Min => _min;

        [SerializeField]
        private int _max;

        public int Max => _max;

        public bool Valid => Min <= Max;

        public IntRangeConfig(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public int GetRandomValue()
        {
            return Valid ? PartyParrotManager.Instance.Random.Next(Min, Max) : 0;
        }

        // rounds down
        public int GetPercentValue(float pct)
        {
            if(!Valid) {
                return 0;
            }

            pct = Mathf.Clamp01(pct);
            return (int)(Min + (pct * (Max - Min)));
        }
    }

    [Serializable]
    public struct FloatRangeConfig
    {
        [SerializeField]
        private float _min;

        public float Min => _min;

        [SerializeField]
        private float _max;

        public float Max => _max;

        public bool Valid => Min <= Max;

        public FloatRangeConfig(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public float GetRandomValue()
        {
            return Valid ? PartyParrotManager.Instance.Random.NextSingle(Min, Max) : 0.0f;
        }

        public float GetPercentValue(float pct)
        {
            if(!Valid) {
                return 0.0f;
            }

            pct = Mathf.Clamp01(pct);
            return Min + (pct * (Max - Min));
        }
    }
}
