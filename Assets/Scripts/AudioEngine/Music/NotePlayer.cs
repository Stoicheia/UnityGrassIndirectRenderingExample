using System;
using System.Collections;
using System.Collections.Generic;
using AudioEngine.Instrument;
using AudioEngine.MusicPlayer;
using AudioEngine.ProceduralAudio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AudioEngine.Music
{
    /// <summary>
    /// A controller for playing 12TET notes to an audio channel.
    /// Analogy: just like a key on a piano.
    /// </summary>
    public class NotePlayer : InstrumentNotePlayerBase
    {
        public event Action<NotePlayer> OnDie;
        
        [SerializeField][ReadOnly] private AudioNoteChannel _channel;
        [field: SerializeField][ReadOnly] public NotePlayerSettings Settings { get; private set; }
        private float _level;
        private Coroutine _currentModulator;
        private float _frequency;
        private float _volume;
        private Waveform _waveform;

        private bool _isActive;
        private bool _isReleased;

        public void Set(AudioNoteChannel channel, NotePlayerSettings settings)
        {
            _channel = channel;
            Settings = settings;
        }

        private void Update()
        {
            if (!_isActive || _channel == null) return;
            _channel.InsertNote(_frequency, _level * _volume, _waveform);
            if (_level < float.Epsilon && _isReleased)
            {
                Die();
            }
        }

        private const float CENT = 1.0005777895065548f; //Mathf.Pow(2, 1.0f/1200);
        public void Play(Note note, float volume, Waveform waveform, float pitchShiftCents = 0)
        {
            _frequency = note.Frequency * Mathf.Pow(CENT, pitchShiftCents);
            _volume = volume;
            _waveform = waveform;
            _isActive = true;
            _isReleased = false;
            OnAttack();
        }

        public void OnAttack()
        {
            if(_currentModulator != null)
                StopCoroutine(_currentModulator);
            _currentModulator = StartCoroutine(Attack());
        }

        private void StartDecay()
        {
            if(_currentModulator != null)
                StopCoroutine(_currentModulator);
            _currentModulator = StartCoroutine(Hold());
        }

        public override void OnRelease()
        {
            if(_currentModulator != null)
                StopCoroutine(_currentModulator);
            _currentModulator = StartCoroutine(Release());
            _isReleased = true;
        }

        public void SetFilters(List<AudioFilter> filters)
        {
            _channel.OrderedFilters = filters;
        }

        private IEnumerator ModulateLevel(float duration, AnimationCurve curve, Action callback = default, float starting = 1)
        {
            float time = 0;

            while (time < duration)
            {
                _level = starting * curve.Evaluate(time/duration);
                time += Time.deltaTime;
                yield return null;
            }
            
            callback?.Invoke();
        }
        
        private IEnumerator Attack()
        {
            return ModulateLevel(Settings.Attack, Settings.AttackCurve, StartDecay);
        }

        private IEnumerator Hold()
        {
            return ModulateLevel(Settings.Decay, Settings.DecayCurve);
        }
        
        private IEnumerator Release()
        {
            return ModulateLevel(Settings.Release, Settings.ReleaseCurve, starting: _level);
        }

        private void Die()
        {
            OnDie?.Invoke(this);
            _isActive = false;
        }
    }

    [Serializable]
    public struct NotePlayerSettings
    {
        [field: SerializeField] public float Attack { get; set; }
        [field: SerializeField] public float Decay { get; set; }
        [field: SerializeField] public float Release { get; set; }
        
        [field: SerializeField] public AnimationCurve AttackCurve { get; set; }
        [field: SerializeField] public AnimationCurve DecayCurve { get; set; }
        [field: SerializeField] public AnimationCurve ReleaseCurve { get; set; }
    }
}