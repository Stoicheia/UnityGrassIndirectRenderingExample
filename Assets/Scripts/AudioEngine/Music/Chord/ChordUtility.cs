using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Utility;

namespace AudioEngine.Music
{
    public static class ChordUtility
    {
        private static Regex chordRegex =
            new Regex(
                @"(?<Accidental>[b#]?)(?<RomanNumeral>IV|VII|VI|III|II|I|V)(?<QualityMarker>[-|o|\+|sus2|sus4]?)(?<Extensions>b6|6|7|maj7|b9|9|#9|11|#11|b13|13)*(?<InversionMarker>[abcdefg]?)"
                );

        public static FunctionalChord ChordNameToChord(string notation)
        {
            Match match = chordRegex.Match(notation);
            CaptureCollection accidentalCapture = match.Groups["Accidental"].Captures;
            CaptureCollection romanCapture = match.Groups["RomanNumeral"].Captures;
            CaptureCollection qualityCapture = match.Groups["QualityMarker"].Captures;
            CaptureCollection extensionsCapture = match.Groups["Extensions"].Captures;
            CaptureCollection inversionCapture = match.Groups["InversionMarker"].Captures;

            FunctionalChord.Sign accidental = accidentalCapture[0].Value switch
            {
                "b" => FunctionalChord.Sign.Flat,
                "#" => FunctionalChord.Sign.Sharp,
                _ => FunctionalChord.Sign.None
            };

            FunctionalChord.TriadDegree degree = Utility.StringToEnum<FunctionalChord.TriadDegree>(romanCapture[0].Value);
            FunctionalChord.Quality quality = qualityCapture[0].Value switch
            {
                "-" => FunctionalChord.Quality.Minor,
                "o" => FunctionalChord.Quality.Diminished,
                "sus2" => FunctionalChord.Quality.Sus2,
                "sus4" => FunctionalChord.Quality.Sus4,
                "+" => FunctionalChord.Quality.Augmented,
                _ => FunctionalChord.Quality.Major
            };

            List<FunctionalChord.Extension> extensions = new List<FunctionalChord.Extension>();
            foreach (Capture extension in extensionsCapture)
            {
                extensions.Add(extension.Value switch
                {
                    "b6" => FunctionalChord.Extension.Flat6,
                    "6" => FunctionalChord.Extension.Major6,
                    "7" => FunctionalChord.Extension.Minor7,
                    "maj7" => FunctionalChord.Extension.Major7,
                    "b9" => FunctionalChord.Extension.Flat9,
                    "9" => FunctionalChord.Extension.Major9,
                    "#9" => FunctionalChord.Extension.Sharp9,
                    "11" => FunctionalChord.Extension.Natural11,
                    "#11" => FunctionalChord.Extension.Sharp11,
                    "b13" => FunctionalChord.Extension.Flat13,
                    "13" => FunctionalChord.Extension.Major13,
                    _ => throw new ArgumentOutOfRangeException()
                });
            }

            FunctionalChord.Inversion inversion = inversionCapture[0].Value switch
            {
                "a" => FunctionalChord.Inversion.First,
                "b" => FunctionalChord.Inversion.Second,
                "c" => FunctionalChord.Inversion.Third,
                "d" => FunctionalChord.Inversion.Fourth,
                "e" => FunctionalChord.Inversion.Fifth,
                "f" => FunctionalChord.Inversion.Sixth,
                _ => FunctionalChord.Inversion.Root
            };

            FunctionalChord fChord = new FunctionalChord()
            {
                Accidental = accidental,
                Degree = degree,
                TriadChordQuality = quality,
                Extensions = extensions,
                ChordInversion = inversion
            };

            return fChord;
        }
        
        public static Chord GetChord(string notation, KeySignature key, ChordVoicingPattern pattern)
        {
            return ChordNameToChord(notation).GetChord(pattern, key);
        }
    }
}