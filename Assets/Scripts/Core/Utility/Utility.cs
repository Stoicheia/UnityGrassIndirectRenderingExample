using System;
using UnityEngine;

namespace Core.Utility
{
    public static class Utility
    {
        public static KeyCode StringToKeyCode(string keyName)
        {
            KeyCode key = (KeyCode) System.Enum.Parse(typeof(KeyCode), keyName);
            return key;
        }

        public static T StringToEnum<T>(string name) where T : Enum
        {
            T @enum = (T) Enum.Parse(typeof(T), name);
            return @enum;
        }
    }

    public static class AudioUtility
    {
        public static float BeatsToSeconds(float beats, float bpm)
        {
            return (beats / bpm) * 60;
        }
    }

    public static class Vector2Utility
    {
        public static Vector2 Polar(float r, float theta)
        {
            return new Vector2(Mathf.Sin(theta), Mathf.Cos(theta)) * r;
        }
    }
}