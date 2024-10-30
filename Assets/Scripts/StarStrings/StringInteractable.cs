using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MagicGrass.StarStrings
{
    [RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
    public class StringInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _name;
        [SerializeField] private List<StringInteractionParams> _callAndResponses;
        // first person sprite data here
        private int _callResponseIndex;

        private AudioSource _audioSource;
        private Rigidbody _rb;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _rb = GetComponent<Rigidbody>();
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
        public void Interact(Interactor interactor)
        {
            Destroy(gameObject);
        }

        public string PromptText => "Press E to Pickup " + _name;

        public void Launch()
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