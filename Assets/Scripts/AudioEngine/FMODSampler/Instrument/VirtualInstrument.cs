using System.Collections.Generic;
using System.Linq;
using AudioEngine.Music;
using AudioEngine.MusicPlayer;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AudioEngine.FMODSampler
{
    /// <summary>
    /// Interface for controlling many notes at once.
    /// Analogy: Just like a virtual piano.
    /// </summary>
    public class VirtualInstrument : VirtualInstrumentBase
    {
        private const int POLYPHONY = 50;
        
        [SerializeField][ReadOnly] private List<InstrumentNoteInstance> _notePlayers;

        [field: Header("Settings")] 
        [field: SerializeField][field: Range(0, 1)] public float Volume01 { get; set; }
        [field: SerializeField] public float MaxHoldTimeSeconds { get; set; }
        
        [Header("Settings (Editor Only)")]
        [SerializeField] private EventReference _fmodEvent;
        [SerializeField][DisableInPlayMode] private NotePlayerSettings _envelope;
        [SerializeField][DisableInPlayMode] private Note _lowestNote;
        [SerializeField][DisableInPlayMode] private Note _highestNote;

        private Dictionary<Note, InstrumentNoteInstance> _preparedNotePlayers;
        private List<Note> _noteRange;

        private void Awake()
        {
            _noteRange = Note.GetNotesBetween(_lowestNote, _highestNote);
            SpawnNotePlayers();
        }

        private void Update()
        {
            PrepareFreeNotePlayers();
            foreach (InstrumentNoteInstance instrumentNoteInstance in _notePlayers)
            {
                instrumentNoteInstance.NotePlayer.GlobalVolume01 = Volume01;
                instrumentNoteInstance.MaxHoldTime = MaxHoldTimeSeconds;
            }
        }

        [Button]
        public override InstrumentNotePlayerBase PlayNote(Note note)
        {
            InstrumentNoteInstance notePlayer;
            if (_preparedNotePlayers.ContainsKey(note))
            {
                notePlayer = _preparedNotePlayers[note];
            }
            else
            {
                if (_preparedNotePlayers.Count == 0)
                {
                    Debug.LogWarning($"<b>Instrument:</b> Polyphony limit exceeded. Max: {POLYPHONY}. Not playing.");
                    return null;
                }
                var stolenInstance = _preparedNotePlayers.First();
                notePlayer = stolenInstance.Value;
                _preparedNotePlayers.Remove(stolenInstance.Key);
                PrepareNotePlayer(notePlayer, note);
            }
            
            notePlayer.Attack();
            _preparedNotePlayers.Remove(note);

            return notePlayer;
        }

        public override void Release(InstrumentNotePlayerBase notePlayer)
        {
            if(notePlayer != null)
                notePlayer.OnRelease();
        }

        [Button]
        private void PlayRandomNote()
        {
            PlayNote(_noteRange[Random.Range(0, _noteRange.Count)]);
        }

        private void SpawnNotePlayers()
        {
            _notePlayers = new List<InstrumentNoteInstance>();
            _preparedNotePlayers = new Dictionary<Note, InstrumentNoteInstance>();
            for (int i = 0; i < POLYPHONY; i++)
            {
                GameObject obj = new GameObject($"Instrument Note Player {i + 1}");
                obj.transform.parent = transform.parent;
                InstrumentNoteInstance noteInstance = obj.AddComponent<InstrumentNoteInstance>();
                GameObject subObj = new GameObject($"Note Audio {i + 1}");
                subObj.transform.parent = obj.transform;
                SingleNotePlayer notePlayer = subObj.AddComponent<SingleNotePlayer>();
                notePlayer.FmodEvent = _fmodEvent;
                noteInstance.NotePlayer = notePlayer;
                noteInstance.Envelope = _envelope;
                noteInstance.Owner = this;
                _notePlayers.Add(noteInstance);
            }
        }

        private void PrepareFreeNotePlayers()
        {
            foreach (Note note in _noteRange)
            {
                if (!_preparedNotePlayers.ContainsKey(note))
                {
                    TryPrepareNotePlayer(note);
                }
            }
        }

        private bool TryPrepareNotePlayer(Note note)
        {
            bool freeInstanceExists = TryGetFreeNoteInstance(out InstrumentNoteInstance freeInstance);
            if (freeInstanceExists)
            {
                freeInstance.SetNote(note);
                _preparedNotePlayers.Add(note, freeInstance);
            }

            return freeInstanceExists;
        }
        
        private void PrepareNotePlayer(InstrumentNoteInstance instance, Note note)
        {
            instance.SetNote(note);
            _preparedNotePlayers.Add(note, instance);
        }
        
        private bool TryGetFreeNoteInstance(out InstrumentNoteInstance freeInstance)
        {
            freeInstance = null;
            foreach (InstrumentNoteInstance noteInstance in _notePlayers)
            {
                if (noteInstance.IsFree)
                {
                    freeInstance = noteInstance;
                    return true;
                }
            }
            return false;
        }
    }
}