using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioEngine.Music
{
    [Serializable]
    public struct ExoticScale
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<int> SharpScaleDegrees;
        [field: SerializeField] public List<int> FlatScaleDegrees;
        [field: SerializeField] public List<int> DoubleSharpScaleDegrees;
        [field: SerializeField] public List<int> DoubleFlatScaleDegrees;

        public int GetShift(NoteLetter noteLetter, KeySignature keySignature)
        {
            int scaleDegree = keySignature.GetScaleDegreeOf(noteLetter);
            if (SharpScaleDegrees.Contains(scaleDegree)) return 1;
            if (FlatScaleDegrees.Contains(scaleDegree)) return -1;
            if (DoubleSharpScaleDegrees.Contains(scaleDegree)) return 2;
            if (DoubleFlatScaleDegrees.Contains(scaleDegree)) return -2;
            return 0;
        }
    }
}