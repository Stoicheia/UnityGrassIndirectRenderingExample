using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace AudioEngine.Music
{
    /// <summary>
    /// Describes which notes of a FunctionalChord are used when it is converted to an actual Chord.
    /// Example: Octave=3, 5, 3, ^, 3, 9
    /// </summary>
    [Serializable]
    public struct ChordVoicingPattern
    {
        [SerializeField] private List<string> _components;
        [field: SerializeField] public int RootOctave { get; private set; }

        public ChordVoicingPattern(string pattern)
        {
            _components = new List<string>();
            RootOctave = 2;
            IEnumerable<string> symbols = pattern.Split(',').Select(x => x.Trim());
            foreach (string symbol in symbols)
            {
                if (symbol.Contains("Octave"))
                {
                    RootOctave = Int32.Parse(symbol.Split('=')[1]);
                }
                else
                {
                    _components.Add(symbol);
                }
            }
        }

        public List<(int degree, int octavesAbove)> GetComponents()
        {
            List<(int, int)> componentsWithOctaves = new List<(int, int)>();
            int fullOctavesAboveBass = 0;
            int lastNumberSymbol = 1;
            for (int i = 0; i < _components.Count; i++)
            {
                string symbol = _components[i];
                if (symbol == "^")
                {
                    fullOctavesAboveBass++;
                    continue;
                }
                int numberSymbol = Int32.Parse(symbol);
                if (numberSymbol <= lastNumberSymbol)
                {
                    fullOctavesAboveBass++;
                }
                lastNumberSymbol = numberSymbol;
                componentsWithOctaves.Add((numberSymbol, fullOctavesAboveBass));
            }

            return componentsWithOctaves;
        }
    }
}