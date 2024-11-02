using System;
using System.Collections.Generic;
using System.Linq;
using AudioEngine.Music;
using Core.Input;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VirtualInstrument = AudioEngine.FMODSampler.VirtualInstrument;

namespace AudioEngine.MusicPlayer
{
    /// <summary>
    /// Allows us to play entire chords at once on a VirtualInstrument by pressing buttons on the keyboard.
    /// Can play one chord at a time.
    /// Does not play individual notes (InstrumentController does that).
    /// </summary>
    public class ChordInstrumentController : SerializedMonoBehaviour
    {
        public FunctionalChord? ActiveChord; 
        public event Action<bool> OnRelease;
        public event Action<FunctionalChord> OnPlayChord;
        
        [Header("Dependencies")]
        [SerializeField] private VirtualInstrument _instrument;
        [Header("Configuration")]
        [OdinSerialize] private string _chordVoicingDefinition;
        [OdinSerialize] private NoteLetter _majorKeyRoot;
        [OdinSerialize] private float _maxHoldTime = 8f;
        [OdinSerialize] private Note _lowestNonBassNote;
        [OdinSerialize] private Note _highestNote;
        [Header("Debug")]
        [OdinSerialize][ReadOnly] private ChordVoicingPattern _chordVoicingPattern;
        [SerializeField][ReadOnly] private KeySignature _keySignature;
        [SerializeField] [ReadOnly] private List<string> _notesBeingPlayed;

        private List<InstrumentNotePlayerBase> _activeChordPlayers;
        private Chord? _lastChord;
        [SerializeField] private Chord _simulatedLastChord;

        private float _timeSinceLastChord;

        private void Awake()
        {
            ParseDefinitions();
            _activeChordPlayers = new List<InstrumentNotePlayerBase>();
            _lastChord = _simulatedLastChord;
            ActiveChord = null;
        }

        private void Update()
        {
            if (Time.time > _timeSinceLastChord + _maxHoldTime)
            {
                ReleaseCurrentChord(false);
            }
        }

        public void PlayChord(FunctionalChord chordDef, bool replace = true)
        {
            if (!replace && chordDef.Equals(ActiveChord))
            {
                return;
            }

            Chord unmodifiedChord = chordDef.GetChord(_chordVoicingPattern, _keySignature);
            Chord chordToPlay;
            Note toBassNote = unmodifiedChord.Notes[0];   
            Chord toChordWithoutBass = Chord.TrimBass(unmodifiedChord);
            
            if (_lastChord == null || _lastChord.Value.IsEmpty())
            {
             
                Chord clampedChord = Chord.ClampChord(toChordWithoutBass, _lowestNonBassNote, _highestNote);
                chordToPlay = clampedChord.AppendNote(toBassNote);
            }
            else
            {
                Chord fromChordWithoutBass = Chord.TrimBass(_lastChord.Value);
                toChordWithoutBass = Chord.TrimBass(unmodifiedChord);
                Chord bestChord = Chord.GetChordWithVoiceLeading(fromChordWithoutBass, toChordWithoutBass, _lowestNonBassNote, _highestNote);
                chordToPlay = bestChord.AppendNote(toBassNote);
            }
                
            ReleaseCurrentChord(true);

            for (int i = 0; i < chordToPlay.Notes.Count; i++)
            {
                var note = chordToPlay.Notes[i];
                _activeChordPlayers.Add(_instrument.PlayNote(note));
            }

            _lastChord = chordToPlay;
            ActiveChord = chordDef;
            _notesBeingPlayed = chordToPlay.Notes.Select(x => x.ToString()).ToList();
            
            OnPlayChord?.Invoke(chordDef);
            _timeSinceLastChord = Time.time;
        }
        
        public void ReleaseCurrentChord(bool countAsChordChange)
        {
            if (_activeChordPlayers != null)
            {
                foreach (var note in _activeChordPlayers)
                {
                    _instrument.Release(note);
                }
            }
            _activeChordPlayers.Clear();
            
            _lastChord = _simulatedLastChord;
            ActiveChord = null;
            
            OnRelease?.Invoke(countAsChordChange);
        }

        [Button]
        private void ParseDefinitions()
        {
            _chordVoicingPattern = new ChordVoicingPattern(_chordVoicingDefinition);
            _keySignature = new KeySignature(_majorKeyRoot);
        }
    }
}