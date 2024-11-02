using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Core.Input
{
    public class ActiveKeybinds : SingletonMonoBehaviour<ActiveKeybinds>
    {
        [field: OdinSerialize] private Dictionary<InputAction, KeyCode> _action2Key;

        public KeyCode GetKey(InputAction action)
        {
            if (_action2Key.TryGetValue(action, out var key))
            {
                return key;
            }
            
            Debug.LogError($"<b>Input:</b> No keybind found for action {action}");
            return KeyCode.Joystick1Button19;
        }
    }
}