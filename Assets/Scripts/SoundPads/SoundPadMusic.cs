using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MagicGrass.SoundPads
{
    public class SoundPadMusic : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter _musicPlayer;
        [SerializeField] private EventReference _music;
        
        [Button]
        public void Play()
        {
            _musicPlayer.EventReference = _music;
            _musicPlayer.Play();
        }
    }
}