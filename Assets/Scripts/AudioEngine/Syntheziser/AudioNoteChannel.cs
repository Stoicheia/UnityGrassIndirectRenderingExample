using System;
using System.Collections.Generic;
using AudioEngine.Music;
using Sirenix.Serialization;
using UnityEngine;

namespace AudioEngine.ProceduralAudio
{
    [Serializable]
    public class AudioNoteChannel
    {
        [field: SerializeField] public float Amplitude { get; set; }
        [field: SerializeField] public float Frequency { get; set; }
        [field: SerializeField] public bool IsFree { get; private set; }
        [field: SerializeField] public Waveform Waveform { get; set; }
        [field: OdinSerialize] public List<AudioFilter> OrderedFilters { get; set; }
        private float _phase;
        private float _phaseOffset;

        private bool _isStopped;

        public AudioNoteChannel()
        {
            Amplitude = 0;
            Frequency = 0;
            IsFree = true;
        }

        public void InsertNote(Note note, float amplitude, Waveform waveform)
        {
            InsertNote(note.Frequency, amplitude, waveform);
        }
        
        public void InsertNote(float note, float amplitude, Waveform waveform)
        {
            Frequency = note;
            Amplitude = amplitude;
            Waveform = waveform;
            IsFree = Amplitude < float.Epsilon;
        }


        public void Reserve(float phaseOffset = 0)
        {
            _isStopped = false;
            IsFree = false;
            Frequency = 0;
            Amplitude = 0.1f;
            _phase = phaseOffset;
            _phaseOffset = phaseOffset;
        }

        public void NewSample()
        {
            _phase = 0;
        }

        public float Next(float sampleRate)
        {
            float nextRaw = NextRaw(sampleRate);
            if (OrderedFilters == null) return nextRaw;
            float filteredValue = nextRaw;
            foreach (AudioFilter filter in OrderedFilters)
            {
                filteredValue = filter.ApplyFilter(filteredValue, sampleRate);
            }
            return filteredValue;
        }

        public float NextRaw(float sampleRate)
        {
            if (_isStopped) return 0;
            float dWave = Frequency / sampleRate;
            _phase += dWave;
            _phase %= 1;
            IsFree = Amplitude < float.Epsilon;
            if (Waveform == Waveform.Sine) return NextSine();
            else if (Waveform == Waveform.Square) return NextSquare();
            else if (Waveform == Waveform.Saw) return NextSawtooth();
            else if (Waveform == Waveform.Triangle) return NextTriangle();
            return NextSine();
        }

        public float NextSine()
        {
            return Amplitude * Mathf.Sin(2 * Mathf.PI * _phase);
        }

        public float NextSquare()
        {
            float threshold = 0.5f; 
            float wave = Mathf.Sin(2 * Mathf.PI * _phase);
            return wave > threshold ? Amplitude : -Amplitude;
        }
        
        public float NextSawtooth()
        {
            float normalizedPhase = _phase - Mathf.Floor(_phase);
            return 2 * Amplitude * (normalizedPhase - 0.5f);
        }
        
        public float NextTriangle()
        {
            if (_phase < 0.25f)
                return 4 * Amplitude * _phase;
            if (_phase < 0.75f)
                return 2 * Amplitude * (0.5f - _phase);
            return 4 * Amplitude * (_phase - 1.0f);
        }

        public void ForceStop()
        {
            _isStopped = true;
        }
    }
}