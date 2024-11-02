using System.Collections.Generic;
using Core.Input;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AudioEngine.Music
{
    [CreateAssetMenu(fileName = "Chord Set", menuName = "Music/Chord Set", order = 0)]
    public class ChordSet : SerializedScriptableObject
    {
        [field: OdinSerialize] public Dictionary<InputAction, FunctionalChord> Input2Chord { get; set; }
    }
}