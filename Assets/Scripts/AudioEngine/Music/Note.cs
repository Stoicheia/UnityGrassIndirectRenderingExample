using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioEngine.Music
{
    /// <summary>
    /// A note in 12-tone equal temperament. Represents one of the frequencies allowed in 12TET. E.g. A5, E#2
    /// </summary>
    [Serializable]
    public struct Note
    {
        [field: SerializeField] public NoteLetter NoteLetter { get; set; }
        [field: SerializeField] public int Octave { get; set; }
        public Note(NoteLetter letter, int octave)
        {
            NoteLetter = letter;
            Octave = octave;
        }

        public Note(string def)
        {
            def = MusicUtility.NormalizeNoteName(def);
            string GetString(IEnumerable<char> subarray)
            {
                return subarray.Aggregate("", (s, c) => s + c);
            }

            int octave = 4;
            NoteLetter letter = NoteLetter.A;
            char[] nameParts = def.ToCharArray();
            for (int i = 0; i < nameParts.Length; i++)
            {
                if (!Int32.TryParse(GetString(nameParts.Skip(i)), out octave))
                {
                    continue;
                }

                letter = MusicUtility.StringToNoteLetter(GetString(nameParts.Take(i)));
            }
            
            NoteLetter = letter;
            Octave = octave;
        }

        public Note ChangeBySemitones(int semitones)
        {
            (int octaveChange, NoteLetter noteLetter) raisedNote = MusicUtility.RaiseNote(NoteLetter, semitones);
            return new Note(raisedNote.noteLetter, Octave + raisedNote.octaveChange);
        }

        public int GetDistanceSemitones(Note otherNote = default)
        {
            int thisDistFromC = Array.IndexOf(MusicUtility.LetterOrder, NoteLetter);
            int otherDistFromC = Array.IndexOf(MusicUtility.LetterOrder, otherNote.NoteLetter);

            int otherOctave = otherNote.Octave;

            int distance = otherDistFromC - thisDistFromC + 12 * (otherOctave - Octave);
            return distance;
        }

        public (int semitones, Note bestNote) GetDistanceInfo(Chord chord)
        {
            Note bestNote = chord.Notes.First();
            int bestDistance = Int32.MaxValue;

            foreach (var note in chord.Notes)
            {
                int distance = Math.Abs(GetDistanceSemitones(note));
                if (distance < bestDistance)
                {
                    bestNote = note;
                    bestDistance = distance;
                }
            }

            return (bestDistance, bestNote);
        }

        public float Frequency => MusicUtility.NoteNameToFrequency(ToString());

        public static List<Note> GetNotesBetween(Note low, Note high)
        {
            List<Note> notes = new List<Note>();
            Note currentNote = low;
            while (low.Octave <= high.Octave)
            {
                if (currentNote.Equals(high)) break;
                notes.Add(currentNote);
                currentNote = currentNote.ChangeBySemitones(1);
            }

            return notes;
        }
        
        public override string ToString()
        {
            return $"{NoteLetter.ToString().Replace('s', '#')}{Octave}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Note)) return false;
            Note other = (Note) obj;
            return MusicUtility.GetIntervalSemitones(this, other) == 0;
        }

        public static bool operator ==(Note a, Note b)
        {
            return MusicUtility.GetIntervalSemitones(a, b) == 0;
        }

        public static bool operator !=(Note a, Note b)
        {
            return MusicUtility.GetIntervalSemitones(a, b) != 0;
        }

        public static bool operator <(Note a, Note b)
        {
            return MusicUtility.GetIntervalSemitones(a, b) > 0;
        }

        public static bool operator >(Note a, Note b)
        {
            return MusicUtility.GetIntervalSemitones(a, b) < 0;
        }
        
        public static bool operator <=(Note a, Note b)
        {
            return MusicUtility.GetIntervalSemitones(a, b) >= 0;
        }

        public static bool operator >=(Note a, Note b)
        {
            return MusicUtility.GetIntervalSemitones(a, b) <= 0;
        }

        public override int GetHashCode()
        {
            return (int)NoteLetter * 101 + Octave;
        }
    }
}