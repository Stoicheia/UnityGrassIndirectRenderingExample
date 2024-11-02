using Sirenix.OdinInspector;

namespace AudioEngine.MusicPlayer
{
    public abstract class InstrumentNotePlayerBase : SerializedMonoBehaviour
    {
        public VirtualInstrumentBase Owner { get; set; }
        public abstract void OnRelease();
    }
}