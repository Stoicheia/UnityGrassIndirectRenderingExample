using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioEngine.Music;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AudioEngine.ProceduralAudio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioGenerator : SerializedMonoBehaviour
    {
        private float _sampleRate;
        [Header("Volume")] [SerializeField] private float _globalVolumeAmplitude;

        [Header("Voices")] [SerializeField] [DisableInPlayMode]
        private int _voicesCapacity;

        [OdinSerialize] private List<AudioNoteChannel> _voices;
        private int _activeVoiceCount => _voices.Count(x => !x.IsFree);

        private void Awake()
        {
            _sampleRate = AudioSettings.outputSampleRate;
            _voices = new List<AudioNoteChannel>(_voicesCapacity);
            for (int i = 0; i < _voicesCapacity; i++)
            {
                _voices.Add(new AudioNoteChannel());
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5)) Kill();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            for (int sample = 0; sample < data.Length; sample += channels)
            {
                float waveValue = _globalVolumeAmplitude * _voices.Where(x => !x.IsFree).Sum(x => x.Next(_sampleRate));

                for (int c = 0; c < channels; c++)
                {
                    data[sample + c] = waveValue;
                }
            }
        }

        public AudioNoteChannel GrabChannel()
        {
            AudioNoteChannel channel = _voices.FirstOrDefault(x => x.IsFree);
            if (channel == null)
            {
                Debug.LogWarning($"Too many voices. Skipping. (Capacity: {_voicesCapacity})");
                return null;
            }

            channel.Reserve();
            return channel;
        }

        public AudioNoteChannel PlayNewVoice(Note note, float amplitude, Waveform waveform)
        {
            AudioNoteChannel channel = _voices.FirstOrDefault(x => x.IsFree);
            if (channel == null)
            {
                Debug.LogWarning($"Too many voices. Skipping. (Capacity: {_voicesCapacity})");
                return null;
            }

            channel.InsertNote(note, amplitude, waveform);
            return channel;
        }

        [Button]
        public void Kill()
        {
            foreach (var noteChannel in _voices)
            {
                noteChannel.Amplitude = 0;
                noteChannel.ForceStop();
            }
        }
    }
}
