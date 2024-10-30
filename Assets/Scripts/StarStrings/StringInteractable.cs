using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicGrass.StarStrings
{
    public class StringInteractable : MonoBehaviour
    {
        [SerializeField] private List<StringInteractionParams> _callAndResponses;
        [SerializeField] private bool _isSequence;
        private int _callResponseIndex;

        public void PlayNext()
        {
            
        }
    }

    [Serializable]
    public struct StringInteractionParams
    {
        [field: SerializeField] public AudioClip Call { get; set; }
        [field: SerializeField] public float Delay { get; set; }
        [field: SerializeField] public AudioClip Response { get; set; }
    }
}