using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace pdxpartyparrot.Core.UI
{
    public sealed class SplashScreen : MonoBehaviour
    {
        [Serializable]
        public struct SplashScreenConfig
        {
            public VideoClip videoClip;

            public float[] volume;
        }

        [SerializeField]
        private SplashScreenConfig[] _splashScreens;

        [SerializeField]
        private string _mainSceneName = "main";

        [SerializeField]
        private UnityEngine.Camera _camera;

        [SerializeField]
        [ReadOnly]
        private int _currentSplashScreen;

        private VideoPlayer _videoPlayer;

#region Unity Lifecycle
        private void Awake()
        {
            _camera.clearFlags = CameraClearFlags.Color;
            _camera.backgroundColor = Color.black;
            _camera.cullingMask = -1;
            _camera.orthographic = true;
            _camera.useOcclusionCulling = false;

            _videoPlayer = _camera.gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.waitForFirstFrame = false;
            _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            _videoPlayer.isLooping = false;
            _videoPlayer.errorReceived += ErrorReceivedEventHandler;
        }

        private void OnDestroy()
        {
            _videoPlayer = null;
        }

        private void Start()
        {
            PlayNextSplashScreen();
        }
#endregion

        private void PlayNextSplashScreen()
        {
            if(_currentSplashScreen >= _splashScreens.Length) {
                Debug.Log($"Loading main scene '{_mainSceneName}'...");
                SceneManager.LoadScene(_mainSceneName);
                return;
            }

            SplashScreenConfig config = _splashScreens[_currentSplashScreen];
            Debug.Log($"Playing splash screen {config.videoClip.name}");

            _videoPlayer.clip = config.videoClip;

            // config the volume for each track
            _videoPlayer.SetDirectAudioVolume(0, 1.0f);
            for(ushort i=0; i<config.volume.Length && i<_videoPlayer.audioTrackCount; ++i) {
                _videoPlayer.SetDirectAudioVolume(i, config.volume[i]);
            }

            // prepare the clip
            _videoPlayer.prepareCompleted += PrepareCompletedEventHandler;
            _videoPlayer.Prepare();
        }

#region Events
        private void ErrorReceivedEventHandler(VideoPlayer source, string message)
        {
            Debug.LogError($"Video player received error: {message}");
        }

        private void PrepareCompletedEventHandler(VideoPlayer source)
        {
            source.prepareCompleted -= PrepareCompletedEventHandler;

            // ready, play the clip
            source.loopPointReached += LoopPointReachedEventHandler;
            source.Play();
        }

        private void LoopPointReachedEventHandler(VideoPlayer source)
        {
            source.loopPointReached -= LoopPointReachedEventHandler;

            // loop to the next splash
            _currentSplashScreen++;
            PlayNextSplashScreen();
        }
#endregion
    }
}
