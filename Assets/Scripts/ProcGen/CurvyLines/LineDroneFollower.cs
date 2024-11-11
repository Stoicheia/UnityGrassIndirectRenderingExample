using System;
using UnityEngine;

namespace MagicGrass.ProcGen.CurvyLines
{
    public class LineDroneFollower : MonoBehaviour
    {
        public LineDroneMovement MainDrone { get; set; }
        public float Offset { get; set; }
        private TrailRenderer _line;

        private void Awake()
        {
            _line = GetComponent<TrailRenderer>();
        }

        public void Update()
        {
            transform.position = MainDrone.transform.position + MainDrone.transform.up * Offset;
            _line.emitting = MainDrone.IsEmitting;
        }

        public void SetColor(Gradient c)
        {
            _line.colorGradient = c;
        }

        public void SetWidth(float w)
        {
            _line.widthMultiplier = w;
        }

        public void SetExpiry(float e)
        {
            _line.time = e;
        }
    }
}