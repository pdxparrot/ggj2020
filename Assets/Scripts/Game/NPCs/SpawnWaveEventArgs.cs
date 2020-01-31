using System;

namespace pdxpartyparrot.Game.NPCs
{
    public class SpawnWaveEventArgs : EventArgs
    {
        public int WaveIndex { get; }

        public bool IsFinalWave { get; }

        public SpawnWaveEventArgs(int waveIndex, bool isFinalWave)
        {
            WaveIndex = waveIndex;
            IsFinalWave = isFinalWave;
        }
    }
}
