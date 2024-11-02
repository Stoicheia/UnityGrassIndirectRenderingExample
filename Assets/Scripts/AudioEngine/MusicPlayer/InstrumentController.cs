using System;
using System.Collections.Generic;
using System.Linq;
using AudioEngine.Instrument;
using AudioEngine.Music;
using Core.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace AudioEngine.MusicPlayer 
{
    /// <summary>
    /// Allows us to play individual notes on a VirtualInstrument by pressing buttons on the keyboard.
    /// Press multiple keys to play chords.
    /// </summary>
    public class InstrumentController : SerializedMonoBehaviour
    {
        [SerializeField] private List<VirtualInstrument> _instruments;
       
        [Header("Auto-Generate")] 
        [SerializeField] private List<string> _orderedKeys;
        [SerializeField] private string _startingNote;
        [Header("Keybinds")]
        [OdinSerialize] private Dictionary<KeyCode, string> _keyToNote;
        [OdinSerialize] private Dictionary<KeyCode, NoteLetter> _keyToKeySignature;
        [OdinSerialize] private Dictionary<KeyCode, ExoticScale> _holdKeyToExoticScale;
        [OdinSerialize] private KeyCode _sharpKey;
        [OdinSerialize] private KeyCode _flatKey;
        [OdinSerialize] private NoteLetter _majorKeySignatureLetter = NoteLetter.C;
        private KeySignature _keySignature;
        private bool _raiseMinorLeadingNote;

        private Dictionary<KeyCode, List<InstrumentNotePlayerBase>> _activeNotes;

        private void Awake()
        {
            GenerateKeybinds();
            _activeNotes = new Dictionary<KeyCode, List<InstrumentNotePlayerBase>>();
            _keySignature = new KeySignature(_majorKeySignatureLetter);
        }


        private void Update()
        { 
            PollKeysForChangingKey();
            PollKeysForPlayingNotes();
        }

        private void PollKeysForPlayingNotes()
        {
            foreach (var kvp in _keyToNote)
            {
                KeyCode key = kvp.Key;
                string noteLetter = kvp.Value;

                string noteName = noteLetter;
                int semitoneShift = 0;
                if (Input.GetKey(_flatKey) && !Input.GetKey(_sharpKey))
                {
                    semitoneShift--;
                }
                else if (Input.GetKey(_sharpKey) && !Input.GetKey(_flatKey))
                {
                    semitoneShift++;
                }
                
                if (Input.GetKeyDown(key) || (Input.GetKey(key) && !_activeNotes.ContainsKey(key)))
                {
                    Note note = new Note(noteName);

                    if (_keySignature.FlatKeys.Contains(note.NoteLetter))
                    {
                        semitoneShift--;
                    }
                    else if (_keySignature.SharpKeys.Contains(note.NoteLetter))
                    {
                        semitoneShift++;
                    }

                    Note noteInKey = note.ChangeBySemitones(semitoneShift);

                    foreach (var exoticInput in _holdKeyToExoticScale)
                    {
                        if (Input.GetKey(exoticInput.Key))
                        {
                            ExoticScale scale = exoticInput.Value;
                            semitoneShift += scale.GetShift(noteInKey.NoteLetter, _keySignature);
                        }
                    }
                    
                    Note shiftedNote = note.ChangeBySemitones(semitoneShift);
                    List<InstrumentNotePlayerBase> player = _instruments.Select(x => x.PlayNote(shiftedNote)).ToList();
                    _activeNotes[key] = player;
                }
                else if(!Input.GetKey(key))
                {
                    if(_activeNotes.ContainsKey(key))
                    {
                        List<InstrumentNotePlayerBase> player = _activeNotes[key];
                        player.Where(x => x != null).ForEach(x => x.Owner.Release(x));
                        _activeNotes.Remove(key);
                    }
                }
            }
        }

        private void PollKeysForChangingKey()
        {
            foreach (var kvp in _keyToKeySignature)
            {
                KeyCode signatureChangeKey = kvp.Key;
                if (!Input.GetKeyDown(signatureChangeKey)) continue;
                NoteLetter newKeyLetter = kvp.Value;
                _majorKeySignatureLetter = newKeyLetter;
                _keySignature = new KeySignature(newKeyLetter);
            }
        }
        
        
        [Button]
        private void GenerateKeybinds()
        {
            _keyToNote.Clear();
            char[] firstOrder = new[] {'C', 'D', 'E', 'F', 'G', 'A', 'B'};

            int keyIndex = Array.IndexOf(firstOrder, _startingNote[0]);
            int octaveIndex = Int32.Parse(_startingNote.TrimStart(firstOrder));

            foreach (var keyName in _orderedKeys)
            {
                if (keyIndex >= firstOrder.Length)
                {
                    keyIndex = 0;
                    octaveIndex++;
                }
                
                KeyCode key = Utility.StringToKeyCode(keyName);
                _keyToNote[key] = $"{firstOrder[keyIndex]}{octaveIndex}";

                keyIndex++;
            }
        }
    }
}