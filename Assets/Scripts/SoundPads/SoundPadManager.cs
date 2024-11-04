using System;
using System.Collections.Generic;
using AudioEngine.Music;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPadManager : SerializedMonoBehaviour
    {
        [field: OdinSerialize] public SoundPadConfig PadConfig { get; private set; }
        [SerializeField] private SoundPadAudioPlayer _audioPlayer;
        [SerializeField] private List<SoundPad> _pads;
        [SerializeField] private bool _reassignPadsOnAwake;

        private void Awake()
        {
            if (_reassignPadsOnAwake)
            {
                _pads = GetChildPads();
            }
        }

        private void OnEnable()
        {
            SoundPad.OnTrigger += HandleTriggerPad;
            SoundPadAudioPlayer.OnCease += HandleChordStop;
        }

        private void OnDisable()
        {
            SoundPad.OnTrigger -= HandleTriggerPad;
            SoundPadAudioPlayer.OnCease -= HandleChordStop;
        }
        
        private void HandleTriggerPad(SoundPad pad)
        {
            DeactivateAll();
            pad.SetState(SoundPadLevel.Active);
            _audioPlayer.Play(pad.Chord);
        }

        private List<SoundPad> GetChildPads()
        {
            List<SoundPad> childPads = new List<SoundPad>();
            SoundPad[] pads = GetComponentsInChildren<SoundPad>();
            foreach (SoundPad child in pads)
            {
                if (child == null) continue;
                childPads.Add(child);
                if(!child.OverrideConfig)
                    child.SetConfig(PadConfig);
            }
            return childPads;
        }

        public SoundPad GetPadByChord(FunctionalChord chord)
        {
            return _pads.Find(x => x.Chord.Equals(chord));
        }
        
        private void HandleChordStop()
        {
            DeactivateAll();
        }

        private void DeactivateAll()
        {
            foreach (var p in _pads)
            {
                if(p.State.Level != SoundPadLevel.Highlight)
                    p.SetState(SoundPadLevel.Inactive);
            }
        }
        
        public void UnhighlightAll()
        {
            foreach (var p in _pads)
            {
                if (p.State.Level == SoundPadLevel.Highlight)
                {
                    p.SetState(SoundPadLevel.Inactive);
                }
            }
        }

        public void DisableAll()
        {
            foreach (var p in _pads)
            {
                p.SetState(SoundPadLevel.Disabled);
                p.IsActive = true;
            }
        }

        public void EnableAll()
        {
            foreach (var p in _pads)
            {
                p.SetState(SoundPadLevel.Inactive);
                p.IsActive = true;
            }
        }
    }

    [Serializable]
    public struct SoundPadConfig
    {
        [OdinSerialize] public Dictionary<SoundPadLevel, Material> PadStateToMaterial;
        [OdinSerialize] public float Bounciness;
    }
}