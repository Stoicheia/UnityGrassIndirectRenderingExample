using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AudioEngine.Music
{
    
    /// <summary>
    /// An abstract description of a chord based on its harmonic function. Can be converted to an actual chord (with notes).
    /// </summary>
    [Serializable]
    public struct FunctionalChord
    {
        [field: SerializeField] public Sign Accidental { get; set; }
        [field: SerializeField] public TriadDegree Degree { get; set; }
        [field: SerializeField] public Quality TriadChordQuality { get; set; }
        
        public List<Extension> Extensions
        {
            get => _extensions ?? new List<Extension>();
            set => _extensions = value;
        }
        [field: SerializeField] public Inversion ChordInversion { get; set; }
        [SerializeField] private List<Extension> _extensions;
        
        [Serializable]
        public enum Sign
        {
            None, Flat, Sharp 
        }

        [Serializable]
        public enum TriadDegree
        {
            I, II, III, IV, V, VI, VII
        }

        [Serializable]
        public enum Quality
        {
            Major, Minor, Diminished, Augmented, Sus2, Sus4
        }

        [Serializable]
        public enum Extension
        {
            Flat6, Major6, Minor7, Major7, Flat9, Major9, Sharp9, Natural11, Sharp11, Flat13, Major13
        }

        [Serializable]
        public enum Inversion
        {
            Root, First, Second, Third, Fourth, Fifth, Sixth
        }

        public FunctionalChord(Sign accidental, TriadDegree degree, Quality triadChordQuality, List<Extension> extensions, Inversion chordInversion)
        {
            Accidental = accidental;
            Degree = degree;
            TriadChordQuality = triadChordQuality;
            _extensions = extensions;
            ChordInversion = chordInversion;
        }
        
        private static Dictionary<TriadDegree, NoteLetter> _rootsInC = new Dictionary<TriadDegree, NoteLetter>()
        {
            {TriadDegree.I, NoteLetter.C},
            {TriadDegree.II, NoteLetter.D},
            {TriadDegree.III, NoteLetter.E},
            {TriadDegree.IV, NoteLetter.F},
            {TriadDegree.V, NoteLetter.G},
            {TriadDegree.VI, NoteLetter.A},
            {TriadDegree.VII, NoteLetter.B}
        };

        public Chord GetChord(ChordVoicingPattern pattern, KeySignature signature)
        {
            Chord chordInC = GetChordInC(pattern);
            List<Note> notesInNewKey = new List<Note>();
            foreach (Note note in chordInC.Notes)
            {
                notesInNewKey.Add(signature.TransposeFromC(note));
            }
            return new Chord(notesInNewKey);
        }

        public Chord GetChordInC(ChordVoicingPattern pattern)
        {
            List<(int, char)> unsortedScaleDegrees = new List<(int, char)>();

            switch (TriadChordQuality)
            {
                case Quality.Major:
                    unsortedScaleDegrees.Add((1, 'x'));
                    unsortedScaleDegrees.Add((3, 'x'));
                    unsortedScaleDegrees.Add((5, 'x'));
                    break;
                case Quality.Minor:
                    unsortedScaleDegrees.Add((1, 'x'));
                    unsortedScaleDegrees.Add((3, 'b'));
                    unsortedScaleDegrees.Add((5, 'x'));
                    break;
                case Quality.Diminished:
                    unsortedScaleDegrees.Add((1, 'x'));
                    unsortedScaleDegrees.Add((3, 'b'));
                    unsortedScaleDegrees.Add((5, 'b'));
                    break;
                case Quality.Augmented:
                    unsortedScaleDegrees.Add((1, 'x'));
                    unsortedScaleDegrees.Add((3, 'x'));
                    unsortedScaleDegrees.Add((5, '#'));
                    break;
                case Quality.Sus2:
                    unsortedScaleDegrees.Add((1, 'x'));
                    unsortedScaleDegrees.Add((2, 'x'));
                    unsortedScaleDegrees.Add((5, 'x'));
                    break;
                case Quality.Sus4:
                    unsortedScaleDegrees.Add((1, 'x'));
                    unsortedScaleDegrees.Add((4, 'x'));
                    unsortedScaleDegrees.Add((5, 'x'));
                    break;
            }

            foreach (Extension extension in Extensions)
            {
                switch (extension)
                {
                    case Extension.Flat6:
                        unsortedScaleDegrees.Add((6, 'b'));
                        break;
                    case Extension.Major6:
                        unsortedScaleDegrees.Add((6, 'x'));
                        break;
                    case Extension.Minor7:
                        unsortedScaleDegrees.Add((7, 'b'));
                        break;
                    case Extension.Major7:
                        unsortedScaleDegrees.Add((7, 'x'));
                        break;
                    case Extension.Flat9:
                        unsortedScaleDegrees.Add((9, 'b'));
                        break;
                    case Extension.Major9:
                        unsortedScaleDegrees.Add((9, 'x'));
                        break;
                    case Extension.Sharp9:
                        unsortedScaleDegrees.Add((9, '#'));
                        break;
                    case Extension.Natural11:
                        unsortedScaleDegrees.Add((11, 'x'));
                        break;
                    case Extension.Sharp11:
                        unsortedScaleDegrees.Add((11, '#'));
                        break;
                    case Extension.Flat13:
                        unsortedScaleDegrees.Add((13, 'b'));
                        break;
                    case Extension.Major13:
                        unsortedScaleDegrees.Add((13, 'x'));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            List<(int, int)> voicingInfo = pattern.GetComponents();
            Dictionary<int, Queue<int>> normalizedVoicingInfo = new Dictionary<int, Queue<int>>(); // separate voices by scale degree
            foreach (var voice in voicingInfo)
            {
                int scaleDegree = voice.Item1;
                int octavesAbove = voice.Item2;
                if (!normalizedVoicingInfo.ContainsKey(scaleDegree))
                {
                    normalizedVoicingInfo[scaleDegree] = new Queue<int>();
                }
                normalizedVoicingInfo[scaleDegree].Enqueue(octavesAbove);
            }
            int rootOctave = pattern.RootOctave;
            List<Note> chordNotes = new List<Note>();

            Note rootNote = new Note(_rootsInC[Degree], rootOctave);
            if (Accidental == Sign.Flat) rootNote = MusicUtility.RaiseNote(rootNote, -1);
            else if (Accidental == Sign.Sharp) rootNote = MusicUtility.RaiseNote(rootNote, 1);
            
            foreach ((int degree, char accidental) scaleDegree in unsortedScaleDegrees)
            {
                int semitones = $"{scaleDegree.accidental}{scaleDegree.degree}" switch
                {
                    "x1" => 0,
                    "x2" => 2,
                    "b3" => 3,
                    "x3" => 4,
                    "x4" => 5,
                    "x5" => 7,
                    "b5" => 6,
                    "b6" => 8,
                    "x6" => 9,
                    "b7" => 10,
                    "x7" => 11,
                    "b9" => 13,
                    "x9" => 14,
                    "#9" => 15,
                    "x11" => 17,
                    "#11" => 18,
                    "b13" => 20,
                    "x13" => 21,
                    _ => throw new ArgumentOutOfRangeException()
                };

                Note blockChordNote = MusicUtility.RaiseNote(rootNote, semitones);
                Note chordNote;

                bool anyAdded = false;
                
                while (normalizedVoicingInfo.ContainsKey(scaleDegree.degree) && normalizedVoicingInfo[scaleDegree.degree].Count != 0)
                {
                    chordNote = MusicUtility.RaiseNote(blockChordNote, 12 * normalizedVoicingInfo[scaleDegree.degree].Dequeue());
                    chordNotes.Add(chordNote);
                    anyAdded = true;
                }

                if (!anyAdded)
                {
                    chordNote = blockChordNote;
                    chordNotes.Add(chordNote);
                }
            }

            Note bassNote = ChordInversion switch
            {
                Inversion.Root => rootNote,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            chordNotes.Add(bassNote);
            return new Chord(chordNotes);
        }

        public override string ToString()
        {
            StringBuilder chordName = new StringBuilder();
            switch (Accidental)
            {
                case Sign.Flat:
                    chordName.Append("b");
                    break;
                case Sign.Sharp:
                    chordName.Append("#");
                    break;
            }

            bool isLowerCase = TriadChordQuality == Quality.Minor || TriadChordQuality == Quality.Diminished;
            string capitalDegree = Degree.ToString();
            string degree = isLowerCase ? capitalDegree.ToLower() : capitalDegree;

            chordName.Append(degree);

            switch (TriadChordQuality)
            {
                case Quality.Sus2:
                    chordName.Append("sus2");
                    break;
                case Quality.Sus4:
                    chordName.Append("sus4");
                    break;
                case Quality.Augmented:
                    chordName.Append("aug");
                    break;
                case Quality.Diminished:
                    chordName.Append("o");
                    break;
            }

            foreach (var extension in Extensions)
            {
                switch (extension)
                {
                    case Extension.Flat6:
                        chordName.Append("b6");
                        break;
                    case Extension.Major6:
                        chordName.Append("6");
                        break;
                    case Extension.Minor7:
                        chordName.Append("7");
                        break;
                    case Extension.Major7:
                        chordName.Append("maj7");
                        break;
                    case Extension.Flat9:
                        chordName.Append("b9");
                        break;
                    case Extension.Major9:
                        chordName.Append("9");
                        break;
                    case Extension.Sharp9:
                        chordName.Append("#9");
                        break;
                    case Extension.Natural11:
                        chordName.Append("11");
                        break;
                    case Extension.Sharp11:
                        chordName.Append("#11");
                        break;
                    case Extension.Flat13:
                        chordName.Append("b13");
                        break;
                    case Extension.Major13:
                        chordName.Append("13");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return chordName.ToString();
        }

        public enum ChordPage
        {
            Diatonic, Relative, Parallel, Unusual
        }
        public ChordPage GetChordPage()
        {
            string chordName = Simplify().ToString();
            string[] regulars = new[] {"I", "ii", "iii", "IV", "V", "vi", "viio"};
            string[] relatives = new[] {"i", "II", "III", "iv", "v", "VI", "VII"};
            string[] parallels = new[] {"bII", "bIII", "bVI", "bVII"};

            if (regulars.Contains(chordName)) return ChordPage.Diatonic;
            if (relatives.Contains(chordName)) return ChordPage.Relative;
            if (parallels.Contains(chordName)) return ChordPage.Parallel;
            return ChordPage.Unusual;
        }

        public FunctionalChord Simplify()
        {
            return new FunctionalChord
            {
                Accidental = Accidental,
                Degree = Degree,
                TriadChordQuality = TriadChordQuality,
                Extensions = new List<Extension>(),
                ChordInversion = Inversion.Root
            };
        }

        public override int GetHashCode()
        {
            int code = (int) (Accidental + 1) * 2 + (int) (Degree + 1) * 3 + (int) (TriadChordQuality + 1) * 5;
            return code;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is FunctionalChord chord)) return false;
            return chord.Accidental == Accidental && chord.Degree == Degree &&
                   chord.TriadChordQuality == TriadChordQuality;
        }
    }

    
}