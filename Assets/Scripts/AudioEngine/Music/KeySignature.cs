using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AudioEngine.Music
{
    [Serializable]
    public struct KeySignature
    {
        [field: SerializeField] public List<NoteLetter> FlatKeys { get; set; }
        [field: SerializeField] public List<NoteLetter> SharpKeys { get; set; }
        [field: SerializeField] public NoteLetter Root { get; set; }
        [field: SerializeField] public NoteLetter MinorRoot { get; set; }

        private static int[] majorScaleSteps = new[] {2, 2, 1, 2, 2, 2, 1};
        public int GetScaleDegreeOf(NoteLetter letter)
        {
            int degree = 0;
            for (int i = 0; i < 7; i++)
            {
                (int _, NoteLetter checkAgainst) = MusicUtility.RaiseNote(Root, degree);
                if (checkAgainst == letter)
                {
                    return i + 1;
                }
                degree += majorScaleSteps[i];
            }

            return -4673058; // obvious error; don't throw exception here
        }
        
        public (int octavesAbove, NoteLetter letter) GetNoteLetter(int scaleDegree, char accidental = 'x')
        {
            if (scaleDegree < 0)
            {
                Debug.LogError("Negative scale degrees not supported.");
                return default;
            }

            int semitonesAbove = 0;
            int scaleIndex = scaleDegree - 1;
            for (int i = 0; i < scaleIndex; i++)
            {
                semitonesAbove += majorScaleSteps[i];
            }

            if (accidental == 'b') semitonesAbove--;
            else if (accidental == '#') semitonesAbove++;
            
            return MusicUtility.RaiseNote(Root, semitonesAbove);
        }

        public Note TransposeUpFromC(Note noteInC)
        {
            int semitonesAbove = Array.IndexOf(MusicUtility.LetterOrder, Root);
            return MusicUtility.RaiseNote(noteInC, semitonesAbove);
        }

        public Note TransposeDownFromC(Note noteInC)
        {
            return MusicUtility.RaiseNote(TransposeUpFromC(noteInC), -12);
        }

        public Note TransposeFromC(Note noteInC)
        {
            int semitonesAbove = Array.IndexOf(MusicUtility.LetterOrder, Root);
            if (semitonesAbove >= 7) semitonesAbove -= 12;
            return MusicUtility.RaiseNote(noteInC, semitonesAbove);
        }

        public KeySignature(NoteLetter keyMajor)
        {
            FlatKeys = new List<NoteLetter>();
            SharpKeys = new List<NoteLetter>();
            Root = keyMajor;
            switch (keyMajor)
            {
                case NoteLetter.C:
                    MinorRoot = NoteLetter.A;
                    break;
                case NoteLetter.F:
                    FlatKeys.Add(NoteLetter.B);
                    MinorRoot = NoteLetter.D;
                    break;
                case NoteLetter.As:
                    FlatKeys.Add(NoteLetter.B);
                    FlatKeys.Add(NoteLetter.E);
                    MinorRoot = NoteLetter.G;
                    break;
                case NoteLetter.Ds:
                    FlatKeys.Add(NoteLetter.B);
                    FlatKeys.Add(NoteLetter.E);
                    FlatKeys.Add(NoteLetter.A);
                    MinorRoot = NoteLetter.C;
                    break;
                case NoteLetter.Gs:
                    FlatKeys.Add(NoteLetter.B);
                    FlatKeys.Add(NoteLetter.E);
                    FlatKeys.Add(NoteLetter.A);
                    FlatKeys.Add(NoteLetter.D);
                    MinorRoot = NoteLetter.F;
                    break;
                case NoteLetter.Cs:
                    FlatKeys.Add(NoteLetter.B);
                    FlatKeys.Add(NoteLetter.E);
                    FlatKeys.Add(NoteLetter.A);
                    FlatKeys.Add(NoteLetter.D);
                    FlatKeys.Add(NoteLetter.G);
                    MinorRoot = NoteLetter.As;
                    break;
                case NoteLetter.Fs:
                    SharpKeys.Add(NoteLetter.F);
                    SharpKeys.Add(NoteLetter.G);
                    SharpKeys.Add(NoteLetter.A);
                    SharpKeys.Add(NoteLetter.C);
                    SharpKeys.Add(NoteLetter.D);
                    SharpKeys.Add(NoteLetter.E);
                    MinorRoot = NoteLetter.Ds;
                    break;
                case NoteLetter.B:
                    SharpKeys.Add(NoteLetter.F);
                    SharpKeys.Add(NoteLetter.C);
                    SharpKeys.Add(NoteLetter.G);
                    SharpKeys.Add(NoteLetter.D);
                    SharpKeys.Add(NoteLetter.A);
                    MinorRoot = NoteLetter.Gs;
                    break;
                case NoteLetter.E:
                    SharpKeys.Add(NoteLetter.F);
                    SharpKeys.Add(NoteLetter.C);
                    SharpKeys.Add(NoteLetter.G);
                    SharpKeys.Add(NoteLetter.D);
                    MinorRoot = NoteLetter.Cs;
                    break;
                case NoteLetter.A:
                    SharpKeys.Add(NoteLetter.F);
                    SharpKeys.Add(NoteLetter.C);
                    SharpKeys.Add(NoteLetter.G);
                    MinorRoot = NoteLetter.Fs;
                    break;
                case NoteLetter.D:
                    SharpKeys.Add(NoteLetter.F);
                    SharpKeys.Add(NoteLetter.C);
                    MinorRoot = NoteLetter.B;
                    break;
                case NoteLetter.G:
                    SharpKeys.Add(NoteLetter.F);
                    MinorRoot = NoteLetter.E;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}