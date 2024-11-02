using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    public class TargetFrameRate : SerializedMonoBehaviour
    {
        [SerializeField] [DisableInPlayMode] private int _targetFrameRate;

        void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
            DontDestroyOnLoad(gameObject);
        }
    }
}
