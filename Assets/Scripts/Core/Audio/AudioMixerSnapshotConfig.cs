using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.Audio
{
    [Serializable]
    public sealed class AudioMixerSnapshotConfig
    {
        [SerializeField]
        private string _name;

        public string Name => _name;

        [SerializeField]
        private float _weight;

        public float Weight => _weight;
    }

    [Serializable]
    public sealed class AudioMixerSnapshotsConfig
    {
        [SerializeField]
        private string _id;

        public string Id => _id;

        [SerializeField]
        private AudioMixerSnapshotConfig[] _snapshots;

        public IReadOnlyCollection<AudioMixerSnapshotConfig> Snapshots => _snapshots;
    }
}
