using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AudioEngine.Music
{
    /// <summary>
    /// Just a set of notes.
    /// </summary>
    [Serializable]
    public struct Chord
    {
        public List<Note> Notes
        {
            get
            {
                SortNotes();
                return _notes;
            }
        }

        [SerializeField]
        private List<Note> _notes;
        
        public Chord(params string[] notes)
        {
            _notes = new List<Note>();
            foreach (var noteName in notes)
            {
                Note note = new Note(noteName);
                _notes.Add(note);
            }
        }
        
        public Chord(params Note[] notes)
        {
            _notes = new List<Note>();
            foreach (var note in notes)
            {
                _notes.Add(note);
            }
        }
        
        public Chord(List<Note> notes)
        {
            _notes = new List<Note>();
            foreach (var note in notes)
            {
                _notes.Add(note);
            }
        }

        public static Chord Empty()
        {
            Chord chord = new Chord();
            chord._notes = new List<Note>();
            return chord;
        }

        public bool IsEmpty() => _notes.Count == 0;

        public void SortNotes()
        {
            _notes = _notes.OrderBy(x => -x.GetDistanceSemitones()).ToList();
        }

        public Chord AppendNote(Note note)
        {
            Chord newChord = new Chord(_notes);
            newChord._notes.Add(note);
            newChord.SortNotes();
            return newChord;
        }

        public override string ToString()
        {
            string listedNotes = Notes.Aggregate("", (s, n) => s.Length == 0 ? n.ToString() : $"{s}, {n.ToString()}");
            return listedNotes;
        }
        
        public static Chord TrimBass(Chord chordWithBass)
        {
            List<Note> notes = chordWithBass.Notes;
            if (notes.Count == 0) return Empty();
            
            List<Note> notesWithoutBass = notes.GetRange(1, notes.Count - 1);
            return new Chord(notesWithoutBass);
        }

        public static Chord ClampChord(Chord chord, Note lowerBound, Note upperBound)
        {
            List<Note> notes = chord.Notes;
            List<Note> newNotes = new List<Note>(); 
            foreach (Note note in notes)
            {
                Note clampedNote = note;
                while (clampedNote < lowerBound)
                {
                    clampedNote = clampedNote.ChangeBySemitones(12);
                }
                while (clampedNote > upperBound)
                {
                    clampedNote = clampedNote.ChangeBySemitones(-12);
                }
                newNotes.Add(clampedNote);
            }

            return new Chord(newNotes);
        }

        public static Chord GetChordWithVoiceLeading(Chord fromChord, Chord toChord, Note lowerBound, Note upperBound)
        {
            if (toChord.Notes.Count == 0)
            {
                throw new EmptyChordException(toChord);
            }
            
            List<Note> notesRemaining = toChord.Notes;
            Chord builtChord = Chord.Empty();

            while (notesRemaining.Count > 0)
            {
                Note bestNote = toChord.Notes[0];
                Note bestModelNote = toChord.Notes[0];
                int bestDistance = Int32.MaxValue;
                foreach (Note modelNote in notesRemaining)
                {
                    Note lowestNote = MusicUtility.GetFirstNoteAbove(modelNote.NoteLetter, lowerBound);
                    Note highestNote = MusicUtility.GetFirstNoteBelow(modelNote.NoteLetter, upperBound);
                    for (Note note = lowestNote; note <= highestNote; note = MusicUtility.RaiseNote(note, 12))
                    {
                        Chord testChord = builtChord.AppendNote(note);
                        int distance = GetDistance(fromChord, testChord);
                        if (distance < bestDistance)
                        {
                            bestModelNote = modelNote;
                            bestNote = note;
                            bestDistance = distance;
                        }
                    }
                }

                notesRemaining.Remove(bestModelNote);
                builtChord = builtChord.AppendNote(bestNote);
            }

            return builtChord;
        }

        public static int GetDistance(Chord fromChord, Chord toChord)
        {
            int fDistance = 0;
            foreach (Note note in fromChord.Notes)
            {
                fDistance += note.GetDistanceInfo(toChord).semitones;
            }

            int tDistance = 0;
            foreach (Note note in toChord.Notes)
            {
                tDistance += note.GetDistanceInfo(fromChord).semitones;
            }

            return Math.Min(fDistance, tDistance);
        }
    }

    public class EmptyChordException : Exception
    {
        public EmptyChordException(Chord chord) : base(chord.ToString())
        {
        }
    }
}