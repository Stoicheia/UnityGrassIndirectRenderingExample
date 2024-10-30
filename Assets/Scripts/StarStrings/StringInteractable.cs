using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MagicGrass.StarStrings
{
    [RequireComponent(typeof(AudioSource))]
    public class StringInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<StringInteractionParams> _callAndResponses;
        private int _callResponseIndex;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayAgainst(StarString starString)
        {
            if (!starString.IsInteractable)
            {
                return;
            }
            StartCoroutine(PlaySequence(_callResponseIndex++, starString));
        }

        public IEnumerator PlaySequence(int index, StarString interacted)
        {
            interacted.Disable();
            int trueIndex = index % _callAndResponses.Count;
            StringInteractionParams toPlay = _callAndResponses[trueIndex];
            _audioSource.PlayOneShot(toPlay.Call);
            yield return new WaitForSeconds(toPlay.Delay);
            interacted.PlayAudio(toPlay.Response);
            interacted.Enable();
        }

        public void Interact()
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