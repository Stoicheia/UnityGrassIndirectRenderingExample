using AudioEngine.Music;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace AudioEngine.FMODSampler
{
    /// <summary>
    /// Many SingleNotePlayer:s comprise a virtual instrument.
    /// Analogy: Like a single key on a piano.
    /// </summary>
    public class SingleNotePlayer : SerializedMonoBehaviour
    {
        private const int MAX_PITCH_INDEX = 59;
        [field: SerializeField] public EventReference FmodEvent { get; set; }
        [field: SerializeField] public float GlobalVolume01 { get; set; }
        public float Volume01
        {
            get => _volume;
            set
            {
                _volume = value;
                if (_instance.isValid())
                {
                    _instance.setVolume(_volume * GlobalVolume01);
                }
            }
        }
        private EventInstance _instance;
        private int _currentPitchIndex;
        private float _volume;

        [Button]
        public void PrepareNewPitch(Note note)
        {
            int pitchIndex = SamplerUtility.NoteToPitchIndex(note);
            if (pitchIndex > MAX_PITCH_INDEX || pitchIndex < 0)
            {
                Debug.LogError($"Note out of range: {note}");
                return;
            }
            PrepareNewPitch(pitchIndex);
        }
        
        private void PrepareNewPitch(int pitchIndex)
        {
            if (!_instance.isValid())
            {
                _instance = RuntimeManager.CreateInstance(FmodEvent);
            }

            _instance.stop(STOP_MODE.IMMEDIATE);
            _instance.setParameterByName("Pitch", pitchIndex);
            _currentPitchIndex = pitchIndex;
            _instance.start();
            _instance.setPaused(true);
        }
        
        [Button]
        public void Play()
        {
            _instance.setPaused(false);
        }

        [Button]
        public void Stop()
        {
            _instance.stop(STOP_MODE.IMMEDIATE);
        }

        [Button]
        public void SetVolume(float v)
        {
            Volume01 = v;
        }
    }
}