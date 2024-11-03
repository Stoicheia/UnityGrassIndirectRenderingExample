using System;
using TMPro;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPadGame : MonoBehaviour
    {
        [SerializeField] private SoundPadManager _manager;
        [SerializeField] private SoundPadMusic _musicPlayer;
        [SerializeField] private SoundPad _firstPad;
        private bool _hasStarted;

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            SoundPad.OnTrigger += HandleTriggerPad;
            SoundPadMusic.OnMusicEnd += HandleMusicEnd;
        }
        
        private void OnDisable()
        {
            SoundPad.OnTrigger -= HandleTriggerPad;
            SoundPadMusic.OnMusicEnd -= HandleMusicEnd;
        }


        public void Init()
        {
            _manager.DisableAll();
            _firstPad.IsActive = true;
            _firstPad.SetState(SoundPadLevel.Highlight);
            _hasStarted = false;
        }

        public void OnFirstTrigger()
        {
            
        }

        public void End()
        {
            Init();
        }
        
        private void HandleMusicEnd()
        {
            End();
        }
        
        private void HandleTriggerPad(SoundPad pad)
        {
            if (_hasStarted || pad != _firstPad) return;
            _hasStarted = true;
            _manager.EnableAll();
            _musicPlayer.Play();
        }
    }
}