using System;
using System.Collections.Generic;
using AudioEngine.Music;
using AudioEngine.MusicPlayer;
using AudioEngine.ProceduralAudio;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.XR;

namespace AudioEngine.Instrument
{
    public class VirtualInstrument : VirtualInstrumentBase
    {
        [Header("Settings")]
        [SerializeField][Range(-80, 6)] private float _volumeDecibels;
        [SerializeField] private Waveform _sound;
        [SerializeField] private NotePlayerSettings _envelopeSettings;
        [SerializeField] private float _pitchShiftCents;
        [OdinSerialize] private List<AudioFilter> _filters;

        [Header("Dependencies")] 
        [SerializeField] private AudioGenerator _audioSource;
        [OdinSerialize] private Queue<NotePlayer> _notePlayers;

        private float _volumeAmplitude => _volumeDecibels < -80 ? 0 : Mathf.Pow(10, _volumeDecibels / 20);

        private void Awake()
        {
            _notePlayers.Clear();
            NotePlayer[] childPlayers = GetComponentsInChildren<NotePlayer>();
            foreach (var player in childPlayers)
            {
                _notePlayers.Enqueue(player);
            }
            ResetFilters();
        }

        public override InstrumentNotePlayerBase PlayNote(Note note)
        {
            AudioNoteChannel playingChannel = _audioSource.GrabChannel();
            bool hasFreePlayer = _notePlayers.TryDequeue(out NotePlayer player);
            if (!hasFreePlayer)
            {
                Debug.LogWarning($"Maximum note count reached. Not playing.");
                return null;
            }
            player.Set(playingChannel, _envelopeSettings);
            player.SetFilters(_filters);
            player.Play(note, _volumeAmplitude, _sound, _pitchShiftCents);
            player.OnDie += HandlePlayerFreed;
            player.Owner = this;
            return player;
        }

        public NotePlayer PlayNoteWithWaveform(Note note, Waveform waveform)
        {
            AudioNoteChannel playingChannel = _audioSource.GrabChannel();
            bool hasFreePlayer = _notePlayers.TryDequeue(out NotePlayer player);
            if (!hasFreePlayer)
            {
                Debug.LogWarning($"Maximum note count reached. Not playing.");
                return null;
            }
            player.Set(playingChannel, _envelopeSettings);
            player.SetFilters(_filters);
            player.Play(note, _volumeAmplitude, waveform, _pitchShiftCents);
            player.OnDie += HandlePlayerFreed;
            player.Owner = this;
            return player;
        }

        public override void Release(InstrumentNotePlayerBase player)
        {
            if(player != null)
                player.OnRelease();
        }

        private void HandlePlayerFreed(NotePlayer player)
        {
            player.OnDie -= HandlePlayerFreed;
            _notePlayers.Enqueue(player);
        }

        [Button]
        private void ResetFilters()
        {
            _notePlayers.ForEach(x => x.SetFilters(_filters));
        }
    }
}