using System;
using Unity.Mathematics;
using UnityEngine;

namespace MagicGrass.StarStrings
{
    public class Interactor : MonoBehaviour
    {
        public IInteractable InRange => _inRange;
        
        [Header("Config")]
        [SerializeField] private float _range;
        [SerializeField] private LayerMask _interactMask;
        [Header("Debug")]
        [SerializeField] private StringInteractable _currentlyHeld;

        private IInteractable _inRange;

        private void Update()
        {
            SearchInteractable();
        }

        public void SearchInteractable()
        {
            bool isInRange = Physics.Raycast(transform.position, transform.forward, out RaycastHit info,
                _range, _interactMask, QueryTriggerInteraction.Collide);
            if (!isInRange)
            {
                _inRange = null;
            }
            else
            {
                Transform t = info.transform;
                StringInteractable pickup = t.GetComponent<StringInteractable>();
                if (pickup != null)
                {
                    DropCurrent();
                    PickUp(pickup);
                }

                StarString starString = t.GetComponent<StarString>();
                if (starString != null)
                {
                    if (_currentlyHeld == null)
                    {
                        // Bzzt
                    }
                    else
                    {
                        _currentlyHeld.PlayAgainst(starString);
                    }
                }
            }
        }

        public void Interact()
        {
            
        }

        public void PickUp(StringInteractable interactable)
        {
            DropCurrent();
            _currentlyHeld = interactable;
        }

        public void DropCurrent()
        {
            if (_currentlyHeld == null) return;
            StringInteractable dropped = Instantiate(_currentlyHeld, transform.position, Quaternion.identity);
            _currentlyHeld = null;
        }
    }
}