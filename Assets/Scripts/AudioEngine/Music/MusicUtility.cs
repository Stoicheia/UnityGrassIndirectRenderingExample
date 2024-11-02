using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioEngine.Music
{
    public static class MusicUtility
    {
        private static readonly Dictionary<NoteLetter, float> LETTER_TO_FREQUENCY_0 = new Dictionary<NoteLetter, float>()
        {
            {NoteLetter.C, 16.35f}, {NoteLetter.Cs, 17.32f}, {NoteLetter.D, 18.35f}, {NoteLetter.Ds, 19.45f},
            {NoteLetter.E, 20.60f}, {NoteLetter.F, 21.83f}, {NoteLetter.Fs, 23.12f}, {NoteLetter.G, 24.50f},
            {NoteLetter.Gs, 25.96f}, {NoteLetter.A, 27.50f}, {NoteLetter.As, 29.14f}, {NoteLetter.B, 30.87f}
        };
        private static readonly char[] LETTER_CHARS = new[] {'C', 'D', 'E', 'F', 'G', 'A', 'B'};
        private const char NATURAL = 'x';
        private const char FLAT = 'b';
        private const char SHARP = '#';
        
        public static Dictionary<string, float> Generate12TETNotes(int minOctave, int maxOctave)
        {
            Dictionary<string, float> noteTable = new Dictionary<string, float>();
            for (int octave = minOctave; octave <= maxOctave; octave++)
            {
                List<string> noteLetters = LETTER_CHARS
                    .SelectMany(x => new string[] {$"{x}", $"{x}x", $"{x}b", $"{x}#"})
                    .ToList();
                foreach (var noteLetter in noteLetters)
                {
                    string noteName = $"{noteLetter}{octave}";
                    float frequency = NoteNameToFrequency(noteName);
                    noteTable.Add(noteName, frequency);
                }
            }

            return noteTable;
        }

        public static string NormalizeNoteName(string name)
        {
            if (name.EndsWith(NATURAL) || name.EndsWith(SHARP) || name.EndsWith(FLAT))
            {
                char sign = name.Last();
                char letter = name.First();
                string octaveString = name.Substring(1, name.Length - 2);
                int octave = Int32.Parse(octaveString);
                if (letter == 'B' && sign == '#') octave++;
                if (letter == 'C' && sign == 'b') octave--;
                name = $"{letter}{sign}{octave}";
            }
            return name;
        }
        
        public static float NoteNameToFrequency(string name)
        {
            name = NormalizeNoteName(name);
            char[] nameParts = name.ToCharArray();

            string noteName;
            int octave;
            
            float GetDefaultAndLogInvalid()
            {
                Debug.LogError($"Invalid note name: {name}");
                return 440;
            }

            string GetString(IEnumerable<char> subarray)
            {
                return subarray.Aggregate("", (s, c) => s + c);
            }

            for (int i = 0; i < nameParts.Length; i++)
            {
                if (!Int32.TryParse(GetString(nameParts.Skip(i)), out octave))
                {
                    continue;
                }

                noteName = GetString(nameParts.Take(i));
                NoteLetter letter = StringToNoteLetter(noteName);
                if (noteName == "B#") octave++;
                if (noteName == "Cb") octave--;
                float frequencyAtOctaveZero = LETTER_TO_FREQUENCY_0[letter];
                float frequency = frequencyAtOctaveZero * Mathf.Pow(2, octave);
                return frequency;
            }

            return GetDefaultAndLogInvalid();
        }

        public static NoteLetter StringToNoteLetter(string noteName)
        {
            if (noteName.Length == 1) noteName += NATURAL;
            else if (noteName.Length > 2)
            {
                Debug.LogWarning($"Invalid note name {noteName}. Only first two characters will be processed.");
            }

            char preLetter = noteName[0];
            char accidental = noteName[1];

            int noteIndex = Array.IndexOf(LETTER_CHARS, preLetter);
            if (accidental == SHARP)
                noteIndex += noteIndex switch
                {
                    6 => -6,
                    2 => 1,
                    _ => 10
                };
            else if (accidental == FLAT)
                noteIndex += noteIndex switch
                {
                    0 => 6,
                    3 => -1,
                    _ => 9
                };

            return (NoteLetter) noteIndex;
        }

        public static NoteLetter[] LetterOrder = new NoteLetter[]
        {
            NoteLetter.C, NoteLetter.Cs, NoteLetter.D, NoteLetter.Ds, NoteLetter.E,
            NoteLetter.F, NoteLetter.Fs, NoteLetter.G, NoteLetter.Gs, NoteLetter.A,
            NoteLetter.As, NoteLetter.B
        };
        public static (int, NoteLetter) RaiseNote(NoteLetter letter, int by)
        {
            int startPos = Array.IndexOf(LetterOrder, letter);
            int finalPos = startPos + by;
            int raisedOctaves = finalPos < 0 ? (finalPos - 11) / 12 : finalPos / 12;
            int noteIndex = (finalPos%12 + 12)%12;
            return (raisedOctaves, LetterOrder[noteIndex]);
        }

        public static Note RaiseNote(Note note, int by)
        {
            (int octavesAbove, NoteLetter letter) raiseInfo = RaiseNote(note.NoteLetter, by);
            return new Note(raiseInfo.letter, raiseInfo.octavesAbove + note.Octave);
        }

        public static Note GetFirstNoteAbove(NoteLetter letter, Note lowerBound)
        {
            Note currentNote = lowerBound;
            while (currentNote.NoteLetter != letter)
            {
                currentNote = RaiseNote(currentNote, 1);
            }

            return currentNote;
        }

        public static Note GetFirstNoteBelow(NoteLetter letter, Note upperBound)
        {
            Note currentNote = upperBound;
            while (currentNote.NoteLetter != letter)
            {
                currentNote = RaiseNote(currentNote, -1);
            }

            return currentNote;
        }
        
        public static int GetIntervalSemitones(Note left, Note right)
        {
            return left.GetDistanceSemitones(right);
        }
    }

    [Serializable]
    public enum NoteLetter
    {
        C = 0, Cs = 10, D = 1, Ds = 11, E = 2, F = 3, Fs = 13, G = 4, Gs = 14, A = 5, As = 15, B = 6
    }
}