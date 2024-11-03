using System;
using AudioEngine.Music;
using AudioEngine.MusicPlayer;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPadAudioPlayer : MonoBehaviour
    {
        public static event Action OnCease;
        [SerializeField] private ChordInstrumentController _chordPlayer;

        private void OnEnable()
        {
            _chordPlayer.OnTimeout += HandleTimeout;
        }
        
        public void Play(FunctionalChord chord)
        {
            _chordPlayer.PlayChord(chord);
        }
        
        private void HandleTimeout()
        {
            OnCease?.Invoke();
        }
    }
}