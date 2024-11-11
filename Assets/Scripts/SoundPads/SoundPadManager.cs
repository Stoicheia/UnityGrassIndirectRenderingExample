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
        private SoundPadGame _activeGame;

        private void Awake()
        {
            if (_reassignPadsOnAwake)
            {
                UpdatePads();
            }
        }

        private void OnEnable()
        {
            SoundPad.OnTrigger += HandleTriggerPad;
            SoundPadAudioPlayer.OnCease += HandleChordStop;
            SoundPadGame.OnStart += HandleGameStart;
        }

        private void OnDisable()
        {
            SoundPad.OnTrigger -= HandleTriggerPad;
            SoundPadAudioPlayer.OnCease -= HandleChordStop;
            SoundPadGame.OnStart -= HandleGameStart;
        }
        
        private void HandleGameStart(SoundPadGame game)
        {
            if (_activeGame != null)
            {
                _activeGame.End();
            }

            _activeGame = game;
        }

        
        private void HandleTriggerPad(SoundPad pad)
        {
            if (!pad.Chord.HasValue) return;
            DeactivateAll();
            pad.SetState(SoundPadLevel.Active);
            if (pad.Chord != null)
            {
                _audioPlayer.Play(pad.Chord.Value);
            }
        }

        public void UpdatePads()
        {
            var pads = GetChildPads();
            _pads = pads;
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

        public void SetActiveGame(SoundPadGame game)
        {
            // TODO
        }
    }

    [Serializable]
    public struct SoundPadConfig
    {
        [OdinSerialize] public Dictionary<SoundPadLevel, Material> PadStateToMaterial;
        [OdinSerialize] public float Bounciness;
    }
}