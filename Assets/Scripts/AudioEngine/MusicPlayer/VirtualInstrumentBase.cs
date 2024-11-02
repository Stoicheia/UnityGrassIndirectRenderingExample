using AudioEngine.Music;
using Sirenix.OdinInspector;

namespace AudioEngine.MusicPlayer
{
    public abstract class VirtualInstrumentBase : SerializedMonoBehaviour
    {
        public abstract InstrumentNotePlayerBase PlayNote(Note note);
        public abstract void Release(InstrumentNotePlayerBase notePlayer);
    }
}