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
        public static Action<SoundPadGame> OnStart;
        
        //[SerializeField] private SoundPadManager _manager;
        [SerializeField] private SoundPadMusic _musicPlayer;
        [SerializeField] private EventReference _song;
        [SerializeField] private SoundPad _firstPad;
        [SerializeField] private float _secondsBetweenSwitch;
        [SerializeField] private List<SoundPad> _correctAnswers;
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
            DisableAll();
            _musicPlayer.Stop();
            _firstPad.IsActive = true;
            _firstPad.SetState(SoundPadLevel.Highlight);
            _hasStarted = false;
        }

        private void DisableAll()
        {
            foreach (var p in _answerSet)
            {
                p.SetState(SoundPadLevel.Disabled);
                p.IsActive = true;
            }
        }

        private void Update()
        {
            if (Time.time - _lastSwitchTime >= _secondsBetweenSwitch && _hasStarted)
            {
                OnRequestSwitchHighlight();
                _lastSwitchTime = _lastSwitchTime + _secondsBetweenSwitch;
            }

            if (_hasStarted)
            {
                HighlightAnswer();
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
            UnhighlightAll();
            SoundPad foundPad = _correctAnswers[_answerIndex%_correctAnswers.Count];
            if (foundPad != null)
            {
                foundPad.SetState(SoundPadLevel.Highlight);
            }
        }
        
        public void UnhighlightAll()
        {
            foreach (var p in _answerSet)
            {
                if (p.State.Level == SoundPadLevel.Highlight)
                {
                    p.SetState(SoundPadLevel.Inactive);
                }
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
                OnStart?.Invoke(this);
                _hasStarted = true;
                EnableAll();
                _musicPlayer.Play(_song);
                _answerIndex = 1;
                _lastSwitchTime = Time.time;
                HighlightAnswer();
            }
        }
        
        public void EnableAll()
        {
            foreach (var p in _answerSet)
            {
                p.SetState(SoundPadLevel.Inactive);
                p.IsActive = true;
            }
        }
    }
}