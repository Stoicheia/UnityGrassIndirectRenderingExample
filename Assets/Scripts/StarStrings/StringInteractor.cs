using System;
using UnityEngine;

namespace MagicGrass.StarStrings
{
    public class StringInteractor : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float _range;
        [SerializeField] private LayerMask _interactableMask;
        [Header("Debug")]
        [SerializeField] private StringInteractable _currentlyHeld;

        private StringInteractable _inRange;

        private void Update()
        {
            SearchInteractable();
        }

        public void SearchInteractable()
        {
            var search = Physics.Raycast(transform.position, transform.forward, _range, );
        }

        public void Interact()
        {
            
        }

        public void PickUp(StringInteractable interactable)
        {
            
        }

        public void Drop()
        {
            _currentlyHeld = null;
        }
    }
}