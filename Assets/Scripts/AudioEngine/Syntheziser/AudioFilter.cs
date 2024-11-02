using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioEngine.ProceduralAudio
{
    public abstract class AudioFilter
    {
        private const int QUEUE_CAPACITY = 16;
        protected float _alpha;
        private List<float> _lastFilteredValues;
        private List<float> _cachedReturnValues;

        public AudioFilter()
        {
            _lastFilteredValues = new List<float>();
            _cachedReturnValues = new List<float>();
        }
        public abstract float ApplyFilter(float currentSample, float sampleRate);
        protected void AppendValue(float value)
        {
            _lastFilteredValues ??= new List<float>();
            if (_lastFilteredValues.Count >= QUEUE_CAPACITY)
            {
                _lastFilteredValues.RemoveAt(0);
            }
            _lastFilteredValues.Add(value);
        }

        protected List<float> GetLastValues(float defaultValue)
        {
            _cachedReturnValues ??= new List<float>();
            _lastFilteredValues ??= new List<float>();
            _cachedReturnValues.Clear();
            foreach (var v in _lastFilteredValues)
            {
                _cachedReturnValues.Add(v);
            }

            while (_cachedReturnValues.Count < QUEUE_CAPACITY)
            {
                _cachedReturnValues.Add(defaultValue);
            }

            return _cachedReturnValues;
        }
    }

    [Serializable]
    public class LowPassFilter : AudioFilter
    {
        [field: SerializeField] public float CutoffFrequency { get; set; } 
        //[field: SerializeField] public float Mix { get; set; }
        private float _sampleRate;
        
        public LowPassFilter(float sampleRate) : base()
        {
            _sampleRate = sampleRate;
            _alpha = CutoffFrequency / (CutoffFrequency + _sampleRate);
        }
        
        public override float ApplyFilter(float currentSample, float sampleRate)
        {
            _sampleRate = sampleRate;
            _alpha = CutoffFrequency / (CutoffFrequency + _sampleRate);
            List<float> lastValues = GetLastValues(currentSample);
            float lastValue = lastValues.Last();
            float filteredValue = _alpha * currentSample + (1 - _alpha) * lastValue;
            AppendValue(filteredValue);
            return lastValue;
        }
    }
}