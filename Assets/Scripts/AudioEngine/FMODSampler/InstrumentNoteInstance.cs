using System;
using AudioEngine.Music;
using AudioEngine.MusicPlayer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace AudioEngine.FMODSampler
{
    /// <summary>
    /// Holds a note that is currently being played by a VirtualInstrument.
    /// The instrument controls this note's volume, pitch, etc.
    /// </summary>
    [Serializable]
    public class InstrumentNoteInstance : InstrumentNotePlayerBase
    {
        enum NoteState
        {
            Free, Reserved, Attack, Hold, Release
        }

        public bool IsFree => _noteState == NoteState.Free;
        [field: SerializeField] public SingleNotePlayer NotePlayer { get; set; }
        [field: SerializeField] public NotePlayerSettings Envelope { get; set; }
        [field: SerializeField] public float MaxHoldTime { get; set; }
        private float _t;
        private float _curveTimeAtRelease;
        [SerializeField] private NoteState _noteState;
        [ReadOnly][OdinSerialize] private Note? _lastPreparedNote;

        private void Awake()
        {
            _noteState = NoteState.Free;
        }

        public void SetNote(Note note)
        {
            _noteState = NoteState.Reserved;
            NotePlayer.PrepareNewPitch(note);
            _lastPreparedNote = note;
        }

        public void Attack()
        {
            _noteState = NoteState.Attack;
            NotePlayer.Play();
            _t = 0;
        }
        public override void OnRelease() => Release(); // eh
        public void Release()
        {
            _noteState = NoteState.Release;
            _curveTimeAtRelease = _t / Envelope.Decay;
            _t = 0;
        }

        public void Stop()
        {
            NotePlayer.Stop();
            _noteState = NoteState.Free;
            _lastPreparedNote = null;
        }

        public void Update()
        {
            switch (_noteState)
            {
                case NoteState.Attack:
                    UpdateAttack();
                    _t += Time.deltaTime;
                    break;
                case NoteState.Hold:
                    UpdateHold();
                    _t += Time.deltaTime;
                    break;
                case NoteState.Release:
                    UpdateRelease();
                    _t += Time.deltaTime;
                    break;
            }
        }

        private void UpdateAttack()
        {
            if (_t >= Envelope.Attack)
            {
                _noteState = NoteState.Hold;
                _t = 0;
                return;
            }

            float curveTime = _t / Envelope.Attack;
            float volume = Envelope.AttackCurve.Evaluate(curveTime);
            NotePlayer.Volume01 = volume;
        }

        private void UpdateHold()
        {
            float curveTime = _t / Envelope.Decay;
            float volume = Envelope.DecayCurve.Evaluate(Mathf.Min(1, curveTime));
            NotePlayer.Volume01 = volume;

            if (_t > MaxHoldTime)
            {
                Release();
            }
        }

        private void UpdateRelease()
        {
            if (_t >= Envelope.Release)
            {
                _t = 0;
                Stop();
            }
            float curveTime = _t / Envelope.Release;
            float volumeMultiplier = Envelope.DecayCurve.Evaluate(Mathf.Min(1, _curveTimeAtRelease));
            float volume = Envelope.ReleaseCurve.Evaluate(curveTime) * volumeMultiplier;
            NotePlayer.Volume01 = volume;
        }
    }
}