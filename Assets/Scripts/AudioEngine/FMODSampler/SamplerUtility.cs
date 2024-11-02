using System;
using AudioEngine.Music;

namespace AudioEngine.FMODSampler
{
    public static class SamplerUtility
    {
        private static readonly NoteLetter[] _fmodLetterOrder = new NoteLetter[]
        {
            NoteLetter.As, NoteLetter.A, NoteLetter.B, NoteLetter.Cs, NoteLetter.C, NoteLetter.Ds, NoteLetter.D,
            NoteLetter.E, NoteLetter.Fs, NoteLetter.F, NoteLetter.Gs, NoteLetter.G
        };
        
        // Assumes alphabetical ordering A#0, A#1, A#2, A#3, A#4, A0, A1, A2, A3, A4, etc. 60 notes in total.
        public static int NoteToPitchIndex(Note note)
        {
            int octave = note.Octave;
            NoteLetter letter = note.NoteLetter;
            int letterIndex = Array.IndexOf(_fmodLetterOrder, letter);
            int pitchIndex = letterIndex * 5 + octave;
            return pitchIndex;
        }
    }
}