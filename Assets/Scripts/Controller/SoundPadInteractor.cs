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
        private float _lastLandTime;

        private void Awake()
        {
            _controller = GetComponent<FirstPersonController>();
        }

        public void LandOnPad(SoundPad pad)
        {
            float jumpForce = _jumpStrengthModifier * pad.Bounciness;
            _controller.RequestForceJump(jumpForce);
            float timeBetweenLands = Time.time - _lastLandTime;
            _lastLandTime = Time.time;
            Debug.Log($"Time between lands: {timeBetweenLands}");
        }
    }

 
}