using System;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MagicGrass.SoundPads
{
    public class SoundPadMusic : MonoBehaviour
    {
        public static event Action OnMusicEnd;
        
        [SerializeField] private StudioEventEmitter _musicPlayer;
        [SerializeField] private EventReference _music;
        [SerializeField] private int _offsetMs = 1202;
        private bool _isPlaying;

        private void Awake()
        {
            var instance = RuntimeManager.CreateInstance(_music);
            instance.getDescription(out var desc);
            desc.loadSampleData();
        }

        [Button]
        public void Play(EventReference song)
        {
            _music = song;
            _musicPlayer.EventReference = _music;
            _musicPlayer.Play();
            Debug.Log(_musicPlayer.EventInstance.setTimelinePosition(_offsetMs));
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