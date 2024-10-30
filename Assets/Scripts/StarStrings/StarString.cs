using UnityEngine;

namespace MagicGrass.StarStrings
{
    [RequireComponent(typeof(AudioSource))]
    public class StarString : MonoBehaviour, IInteractable
    {
        public bool IsInteractable
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }
        
        private AudioSource _audioSource;
        private bool _isInteractable;

        public void PlayAudio(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }

        public void Disable()
        {
            _isInteractable = false;
        }

        public void Enable()
        {
            _isInteractable = true;
        }

        public void Interact(Interactor interactor)
        {
            
        }
        
        public string PromptText => "Press E to Use String";
    }
}