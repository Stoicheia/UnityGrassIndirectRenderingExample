using System;
using UnityEngine;

namespace Core
{
    public class PhysicsModule : MonoBehaviour
    {
        public float UniversalGravity;

        private void Update()
        {
            Physics.gravity = UniversalGravity * Vector3.down;
        }
    }
}