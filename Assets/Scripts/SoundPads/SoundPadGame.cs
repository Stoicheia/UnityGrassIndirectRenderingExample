using System;
using System.Collections.Generic;
using AudioEngine.Music;
using FMODUnity;
using TMPro;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPadGame : MonoBehaviour
    {
        [SerializeField] private SoundPadManager _manager;
        [SerializeField] private SoundPadMusic _musicPlayer;
        [SerializeField] private EventReference _song;
        [SerializeField] private SoundPad _firstPad;
        [SerializeField] private float _secondsBetweenSwitch;
        [SerializeField] private List<FunctionalChord> _correctAnswers;
        [SerializeField] private List<SoundPad> _answerSet;
        
        private bool _hasStarted;
        private float _lastSwitchTime;
        private int _answerIndex;

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

        private void Update()
        {
            if (Time.time - _lastSwitchTime >= _secondsBetweenSwitch && _hasStarted)
            {
                OnRequestSwitchHighlight();
                _lastSwitchTime = _lastSwitchTime + _secondsBetweenSwitch;
            }
        }

        private void OnRequestSwitchHighlight()
        {
            _answerIndex++;
            HighlightAnswer(); 
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

        private void HighlightAnswer()
        {
            _manager.UnhighlightAll();
            FunctionalChord chord = _correctAnswers[_answerIndex%_correctAnswers.Count];
            SoundPad foundPad = _answerSet.Find(x => x.Chord.Equals(chord));
            if (foundPad != null)
            {
                foundPad.SetState(SoundPadLevel.Highlight);
            }
        }
        
        private void HandleTriggerPad(SoundPad pad)
        {
            if (_hasStarted)
            {
                HighlightAnswer();
            }
            else
            {
                if (_hasStarted || pad != _firstPad) return;
                _hasStarted = true;
                _manager.EnableAll();
                _musicPlayer.Play(_song);
                _answerIndex = 1;
                _lastSwitchTime = Time.time;
                HighlightAnswer();
            }
        }
    }
}