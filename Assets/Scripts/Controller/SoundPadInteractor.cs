using System;
using MagicGrass.SoundPads;
using UnityEngine;

namespace MagicGrass.Controller
{
    [RequireComponent(typeof(FirstPersonController))]
    public class SoundPadInteractor : MonoBehaviour, ISoundPadInteractor
    {
        private FirstPersonController _controller;
        [SerializeField] private float _jumpStrengthModifier;

        private void Awake()
        {
            _controller = GetComponent<FirstPersonController>();
        }

        public void LandOnPad(SoundPad pad)
        {
            _controller.Jump(_jumpStrengthModifier);
        }
    }

 
}