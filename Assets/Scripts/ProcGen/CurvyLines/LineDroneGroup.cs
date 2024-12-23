﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicGrass.ProcGen.CurvyLines
{
    public class LineDroneGroup : MonoBehaviour
    {
        [SerializeField] private LineDroneMovement _mainDrone;
        [SerializeField] private List<Gradient> _gradients;
        [SerializeField] private float _offset;
        [SerializeField] private float _thickness = 10;
        [SerializeField] private float _expiryTime = 2;

        [SerializeField] private List<LineDroneFollower> _followers;

        private void Start()
        {
            for (int i = 0; i < _followers.Count; i++)
            {
                float nI = i - (_followers.Count-1)/2.0f;
                float myOffset = _offset * nI;
                LineDroneFollower follower = _followers[i];
                follower.MainDrone = _mainDrone;
                follower.Offset = myOffset;
                follower.SetColor(_gradients[i]);
                follower.SetWidth(_thickness);
            }
        }

        private void Update()
        {
            for (int i = 0; i < _followers.Count; i++)
            {
                LineDroneFollower follower = _followers[i];
                follower.SetColor(_gradients[i]);
                follower.SetWidth(_thickness);
                follower.SetExpiry(_expiryTime);
            }
        }
    }
}