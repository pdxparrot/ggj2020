using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace pdxpartyparrot.Core.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        // config keys
        private const string MasterVolumeKey = "audio.volume.master";
        private const string MusicVolumeKey = "audio.volume.music";
        private const string SFXVolumeKey = "audio.volume.sfx";
        private const string AmbientVolumeKey = "audio.volume.ambient";

        private struct InternalAudioMixerSnapshotConfig
        {
            public AudioMixerSnapshot[] snapshots;
            public float[] weights;
        }

        [SerializeField]
        private AudioData _audioData;

        [Space(10)]

#region SFX
        [Header("SFX")]

        [SerializeField]
        private AudioSource _oneShotAudioSource;
#endregion

        [Space(10)]

#region Stingers
        [Header("Stingers")]

        [SerializeField]
        private AudioSource _stingerAudioSource;

        public bool IsStingerPlaying => _stingerAudioSource.isPlaying;
#endregion

        [Space(10)]

#region Music
        [Header("Music")]

        [SerializeField]
        private AudioSource _music1AudioSource;

        public bool IsMusic1Playing => _music1AudioSource.isPlaying;

        [SerializeField]
        private AudioSource _music2AudioSource;

        public bool IsMusic2Playing => _music2AudioSource.isPlaying;

        public bool IsMusicPlaying => IsMusic1Playing || IsMusic2Playing;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _musicCrossFade;

        // 0 == music1, 1 = music2
        public float MusicCrossFade
        {
            get => _musicCrossFade;
            set => _musicCrossFade = Mathf.Clamp01(value);
        }
#endregion

        [Space(10)]

#region Ambient
        [Header("Ambient")]

        [SerializeField]
        private AudioSource _ambientAudioSource;

        public bool IsAmbientPlaying => _ambientAudioSource.isPlaying;
#endregion

        [Space(10)]

#region Volume
        [Header("Volume")]

        [SerializeField]
        [ReadOnly]
        private bool _mute;

        public bool Mute
        {
            get => _mute;

            set
            {
                _mute = value;
                _mixer.SetFloat(_audioData.MasterVolumeParameter, _mute ? 0.0f : MasterVolume);
            }
        }

        public float MasterVolume
        {
            get => PartyParrotManager.Instance.GetFloat(MasterVolumeKey, _mixer.GetFloatOrDefault(_audioData.MasterVolumeParameter));

            set
            {
                value = Mathf.Clamp(value, -80.0f, 20.0f);

                _mixer.SetFloat(_audioData.MasterVolumeParameter, value);
                PartyParrotManager.Instance.SetFloat(MasterVolumeKey, value);

                Mute = false;
            }
        }

        public float MusicVolume
        {
            get => PartyParrotManager.Instance.GetFloat(MusicVolumeKey, _mixer.GetFloatOrDefault(_audioData.MusicVolumeParameter, -5.0f));

            set
            {
                value = Mathf.Clamp(value, -80.0f, 20.0f);

                _mixer.SetFloat(_audioData.MusicVolumeParameter, value);
                PartyParrotManager.Instance.SetFloat(MusicVolumeKey, value);

                Mute = false;
            }
        }

        public float SFXVolume
        {
            get => PartyParrotManager.Instance.GetFloat(SFXVolumeKey, _mixer.GetFloatOrDefault(_audioData.SFXVolumeParameter));

            set
            {
                value = Mathf.Clamp(value, -80.0f, 20.0f);

                _mixer.SetFloat(_audioData.SFXVolumeParameter, value);
                PartyParrotManager.Instance.SetFloat(SFXVolumeKey, value);

                Mute = false;
            }
        }

        public float AmbientVolume
        {
            get => PartyParrotManager.Instance.GetFloat(AmbientVolumeKey, _mixer.GetFloatOrDefault(_audioData.AmbientVolumeParameter, -10.0f));

            set
            {
                value = Mathf.Clamp(value, -80.0f, 20.0f);

                _mixer.SetFloat(_audioData.AmbientVolumeParameter, value);
                PartyParrotManager.Instance.SetFloat(AmbientVolumeKey, value);

                Mute = false;
            }
        }
#endregion

        private AudioMixer _mixer;

        private readonly Dictionary<string, InternalAudioMixerSnapshotConfig> _snapshotConfigs = new Dictionary<string, InternalAudioMixerSnapshotConfig>();

        private Coroutine _musicTransitionRoutine;

#region Unity Lifecycle
        private void Awake()
        {
            Debug.Assert(_audioData.UpdateCrossfadeUpdateSeconds < _audioData.UpdateMusicTransitionSeconds);

            _mixer = _audioData.Mixer;

            // load our snapshots
            foreach(AudioMixerSnapshotsConfig snapshotsConfig in _audioData.Snapshots) {
                AddSnapshotConfig(snapshotsConfig);
            }

            // add in the paused / unpaused config
            AddSnapshotConfig("unpaused", new InternalAudioMixerSnapshotConfig
            {
                snapshots = new[] { _mixer.FindSnapshot(_audioData.UnpausedSnapshotName) },
                weights = new[] { 1.0f }
            });

            AddSnapshotConfig("paused", new InternalAudioMixerSnapshotConfig
            {
                snapshots = new[] { _mixer.FindSnapshot(_audioData.PausedSnapshotName) },
                weights = new[] { 1.0f }
            });

            // init our audio sources
            InitSFXAudioMixerGroup(_oneShotAudioSource);

            InitSFXAudioMixerGroup(_stingerAudioSource);
            _stingerAudioSource.loop = false;

            InitAudioMixerGroup(_music1AudioSource, _audioData.MusicMixerGroupName);
            _music1AudioSource.loop = true;

            InitAudioMixerGroup(_music2AudioSource, _audioData.MusicMixerGroupName);
            _music2AudioSource.loop = true;

            InitAmbientAudioMixerGroup(_ambientAudioSource);
            _ambientAudioSource.loop = true;

            // this ensures we've loaded the volumes from the config
            MasterVolume = MasterVolume;
            MusicVolume = MusicVolume;
            SFXVolume = SFXVolume;
            AmbientVolume = AmbientVolume;

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;

            InitDebugMenu();
        }

        private void Start()
        {
            StartCoroutine(UpdateMusicCrossfadeRoutine());
        }

        protected override void OnDestroy()
        {
            StopAllAudio();

            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }

            base.OnDestroy();
        }
#endregion

        public void InitSFXAudioMixerGroup(AudioSource source)
        {
            InitAudioMixerGroup(source, _audioData.SFXMixerGroupName);
        }

        private void InitAudioMixerGroup(AudioSource source, string mixerGroupName)
        {
            var mixerGroups = _mixer.FindMatchingGroups(mixerGroupName);
            source.outputAudioMixerGroup = mixerGroups.Length > 0 ? mixerGroups[0] : _mixer.outputAudioMixerGroup;
        }

        public void InitAmbientAudioMixerGroup(AudioSource source)
        {
            InitAudioMixerGroup(source, _audioData.AmbientMixerGroupName);
        }

        public void StopAllAudio()
        {
            StopStinger();

            StopAllMusic();

            StopAmbient();
        }

#region SFX
        public void PlayOneShot(AudioClip audioClip)
        {
            _oneShotAudioSource.PlayOneShot(audioClip);
        }
#endregion

#region Stingers
        public void PlayStinger(AudioClip stingerAudioClip)
        {
            _stingerAudioSource.clip = stingerAudioClip;
            _stingerAudioSource.Play();
        }

        public void StopStinger()
        {
            _stingerAudioSource.Stop();
        }
#endregion

#region Music
        // plays a music clip on the first audio source at no crossfade
        public void PlayMusic(AudioClip musicAudioClip)
        {
            StopAllMusic();

            _music1AudioSource.clip = musicAudioClip;
            _music1AudioSource.Play();

            MusicCrossFade = 0.0f;
        }

        // plays a music clip on the second audio source at full crossfade
        public void PlayMusic2(AudioClip musicAudioClip)
        {
            StopAllMusic();

            _music2AudioSource.clip = musicAudioClip;
            _music2AudioSource.Play();

            MusicCrossFade = 1.0f;
        }

        // plays 2 music clips at 50% crossfade
        public void PlayMusic(AudioClip music1AudioClip, AudioClip music2AudioClip)
        {
            StopAllMusic();

            _music1AudioSource.clip = music1AudioClip;
            _music1AudioSource.Play();

            _music2AudioSource.clip = music2AudioClip;
            _music2AudioSource.Play();

            MusicCrossFade = 0.5f;
        }

        // if stopOnComplete is true, will stop the clip being transitioned away from
        public void TransitionMusicAsync(AudioClip musicAudioClip, float seconds, bool stopOnComplete=true)
        {
            if(null == musicAudioClip) {
                return;
            }

            if(_music1AudioSource.isPlaying && _music2AudioSource.isPlaying) {
                Debug.LogWarning("Attempt to transition music with 2 clips playing");
                return;
            }

            bool transitionToSecond;
            float targetCrossfade = 1.0f - _musicCrossFade;
            if(_music1AudioSource.isPlaying) {
                PlayMusic2(musicAudioClip);
                transitionToSecond = true;
            } else {
                PlayMusic(musicAudioClip);
                transitionToSecond = false;
            }

            _musicTransitionRoutine = StartCoroutine(MusicTransitionRoutine(targetCrossfade, seconds, () => {
                if(stopOnComplete) {
                    if(transitionToSecond) {
                        StopMusic();
                    } else {
                        StopMusic2();
                    }
                }
            }));
        }

        public void StopMusic()
        {
            _music1AudioSource.Stop();
        }

        public void StopMusic2()
        {
            _music2AudioSource.Stop();
        }

        public void StopAllMusic()
        {
            if(null != _musicTransitionRoutine) {
                StopCoroutine(_musicTransitionRoutine);
                _musicTransitionRoutine = null;
            }

            StopMusic();
            StopMusic2();

            StopStinger();
        }

        private IEnumerator UpdateMusicCrossfadeRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(_audioData.UpdateCrossfadeUpdateSeconds);
            while(true) {
                _music1AudioSource.volume = 1.0f - _musicCrossFade;
                _music2AudioSource.volume = _musicCrossFade;

                yield return wait;
            }
        }

        // TODO: not 100% sure this works correctly
        private IEnumerator MusicTransitionRoutine(float targetCrossfade, float seconds, Action onComplete)
        {
            if(seconds <= 0.0f) {
                yield break;
            }

            float timeRemaining = seconds;
            float step = Mathf.Clamp01(_audioData.UpdateMusicTransitionSeconds / seconds) * Mathf.Abs(targetCrossfade - MusicCrossFade);

            WaitForSeconds wait = new WaitForSeconds(_audioData.UpdateMusicTransitionSeconds);
            while(true) {
                MusicCrossFade = Mathf.MoveTowards(MusicCrossFade, targetCrossfade, step);
                yield return wait;

                timeRemaining -= _audioData.UpdateMusicTransitionSeconds;
                if(timeRemaining <= 0.0f) {
                    MusicCrossFade = targetCrossfade;
                    break;
                }
            }

            _musicTransitionRoutine = null;
            onComplete?.Invoke();
        }
#endregion

#region Ambient
        public void PlayAmbient(AudioClip audioClip)
        {
            StopAmbient();

            _ambientAudioSource.clip = audioClip;
            _ambientAudioSource.Play();
        }

        public void StopAmbient()
        {
            _ambientAudioSource.Stop();
        }
#endregion

#region  Snapshots
        private void AddSnapshotConfig(AudioMixerSnapshotsConfig snapshotsConfig)
        {
            InternalAudioMixerSnapshotConfig snapshotConfig = new InternalAudioMixerSnapshotConfig
            {
                snapshots = new AudioMixerSnapshot[snapshotsConfig.Snapshots.Count],
                weights = new float[snapshotsConfig.Snapshots.Count]
            };

            for(int i=0; i<snapshotsConfig.Snapshots.Count; ++i) {
                snapshotConfig.snapshots[i] = _mixer.FindSnapshot(snapshotsConfig.Snapshots.ElementAt(i).Name);
                snapshotConfig.weights[i] = snapshotsConfig.Snapshots.ElementAt(i).Weight;
            }

            AddSnapshotConfig(snapshotsConfig.Id, snapshotConfig);
        }

        private void AddSnapshotConfig(string id, InternalAudioMixerSnapshotConfig snapshotConfig)
        {
            if(_snapshotConfigs.ContainsKey(id)) {
                Debug.LogWarning($"Overwriting snapshot config {id}!");
            }
            _snapshotConfigs.Add(id, snapshotConfig);
        }

        public void SetSnapshots(string snapshotId)
        {
            if(_snapshotConfigs.TryGetValue(snapshotId, out var snapshotConfig)) {
                _mixer.TransitionToSnapshots(snapshotConfig.snapshots, snapshotConfig.weights, 0.1f);
            }
        }
#endregion

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            SetSnapshots(PartyParrotManager.Instance.IsPaused ? "paused" : "unpaused");
        }
#endregion

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.AudioManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Volume", GUI.skin.box);
                    GUILayout.Label($"Master Volume: {MasterVolume}");
                    GUILayout.Label($"Music Volume: {MusicVolume}");
                    GUILayout.Label($"SFX Volume: {SFXVolume}");
                    GUILayout.Label($"Ambient Volume: {AmbientVolume}");
                    GUILayout.Label($"Mute: {Mute}");
                GUILayout.EndVertical();

                GUILayout.BeginVertical("Music", GUI.skin.box);
                    GUILayout.Label($"Music Crossfade: {MusicCrossFade}");
                    GUILayout.Label($"Transitioning: {null != _musicTransitionRoutine}");
                GUILayout.EndVertical();
            };
        }
    }
}
