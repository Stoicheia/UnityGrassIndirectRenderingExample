using System;
using System.Collections.Generic;
using AudioEngine.Music;
using MagicGrass.Controller;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPad : SerializedMonoBehaviour
    {
        public static Action<SoundPad, SoundPadState> OnSetState;
        public static Action<SoundPad> OnTrigger;

        public FunctionalChord? Chord
        {
            get => _chord;
            set => _chord = value;
        }
        public float Bounciness => _bounciness;
        public SoundPadState State => _state;
        [field: SerializeField, ReadOnly] public bool IsActive { get; set; }

        public bool OverrideConfig;
        [SerializeField] private List<MeshRenderer> _renderers;
        [OdinSerialize] private Dictionary<SoundPadLevel, Material> _stateToMaterial;
        [OdinSerialize] private float _bounciness = 1;
        [OdinSerialize] private FunctionalChord? _chord;
        [SerializeField][ReadOnly] private SoundPadState _state;
        
        public void SetState(SoundPadLevel level)
        {
            _state = new SoundPadState()
            {
                IsActive = level == SoundPadLevel.Inactive,
                Level = level
            };
            UpdateMaterial();
            
            OnSetState?.Invoke(this, _state);
        }

        private void UpdateMaterial()
        {
            SoundPadLevel _padLevel = _state.Level;
            foreach (MeshRenderer r in _renderers)
            {
                r.material = _stateToMaterial[_padLevel];
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            if (!IsActive) return;
            Vector3 collisionNormal = col.GetContact(0).normal;
            if (collisionNormal.y >= -0.9f)
            {
                return;
            }
            GameObject against = col.gameObject;
            ISoundPadInteractor interactor = against.GetComponent<ISoundPadInteractor>();
            if (interactor == null) return;
            
            interactor.LandOnPad(this);
            RequestActivate();
        }

        private void RequestActivate()
        {
            OnTrigger?.Invoke(this);
        }

        public void SetConfig(SoundPadConfig config)
        {
            _stateToMaterial = config.PadStateToMaterial;
            _bounciness = config.Bounciness;
        }
    }

    [Serializable]
    public struct SoundPadState
    {
        public bool IsActive;
        public SoundPadLevel Level;
    }

    [Serializable]
    public enum SoundPadLevel
    {
        Disabled, Inactive, Highlight, Active
    }
}