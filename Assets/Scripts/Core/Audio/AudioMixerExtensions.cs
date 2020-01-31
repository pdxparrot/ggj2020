using UnityEngine.Audio;

namespace pdxpartyparrot.Core.Audio
{
    public static class AudioMixerExtensions
    {
        public static float GetFloatOrDefault(this AudioMixer audioMixer, string name, float defaultValue=0.0f)
        {
            return audioMixer.GetFloat(name, out float value) ? value : defaultValue;
        }
    }
}
