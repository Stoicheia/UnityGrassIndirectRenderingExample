using System;
using System.Collections.Generic;
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
            pad.SetState(SoundPadLevel.Good);
            _audioPlayer.Play(pad.Chord);
        }

        private List<SoundPad> GetChildPads()
        {
            List<SoundPad> childPads = new List<SoundPad>();
            foreach (Transform child in transform)
            {
                SoundPad pad = child.GetComponent<SoundPad>();
                if (pad == null) continue;
                childPads.Add(pad);
                pad.SetConfig(PadConfig);
            }
            return childPads;
        }
        
        private void HandleChordStop()
        {
            DeactivateAll();
        }

        private void DeactivateAll()
        {
            foreach (var p in _pads)
            {
                p.SetState(SoundPadLevel.Inactive);
            }
        }
    }

    [Serializable]
    public struct SoundPadConfig
    {
        [OdinSerialize] public Dictionary<SoundPadLevel, Material> PadStateToMaterial;
    }
}