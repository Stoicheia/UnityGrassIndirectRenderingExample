using System;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPadMusic : MonoBehaviour
    {
        public static event Action OnMusicEnd;
        
        [SerializeField] private StudioEventEmitter _musicPlayer;
        [SerializeField] private EventReference _music;
        [SerializeField] private int _offsetMs = 1202;
        private bool _isPlaying;
        
        [Button]
        public void Play()
        {
            _musicPlayer.EventReference = _music;
            _musicPlayer.Play();
            _musicPlayer.Stop();
            _musicPlayer.EventInstance.setTimelinePosition(_offsetMs);
            _musicPlayer.Play();
            _isPlaying = true;
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (!_musicPlayer.IsPlaying())
            {
                _isPlaying = false;
                OnMusicEnd?.Invoke();
            }
        }
    }
}