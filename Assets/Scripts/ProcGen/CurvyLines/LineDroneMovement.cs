using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicGrass.ProcGen.CurvyLines
{
    public class LineDroneMovement : MonoBehaviour
    {
        [SerializeField] private List<Transform> _bezierWaypoints;
        [SerializeField] private float _curveMoveSpeed;
        private float _currentT;
        private TrailRenderer _trail;
        public bool IsEmitting => _trail.emitting;

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _trail.emitting = false;
        }

        private void Update()
        {
            Move(10);
        }

        private void Move(float offset)
        {
            _currentT += Time.deltaTime * _curveMoveSpeed;
            if ((GetSection(_currentT) + 1) * 2 >= _bezierWaypoints.Count)
            {
                _trail.emitting = false;
                _currentT = 0;
            }
            
            if (_currentT > 0.02f)
            {
                _trail.emitting = true;
            }

            if (_currentT > 0.98f)
            {
                _trail.emitting = false;
            }
        
            Vector3 newPos = GetPosition(_currentT);
            Vector3 oldPos = transform.position;
            Vector3 velocity = newPos - oldPos;
            transform.LookAt(newPos);
            transform.Translate(newPos - oldPos, Space.World);
        }

        private Vector3 GetPosition(float t)
        {
            int section = GetSection(t);
            if ((section + 1) * 2 >= _bezierWaypoints.Count)
            {
                return Vector3.zero;
            }

            float lT = t - section;
            Vector3 leftPoint = _bezierWaypoints[section*2].position;
            Vector3 midPoint = _bezierWaypoints[section*2 + 1].position;
            Vector3 rightPoint = _bezierWaypoints[(section+1)*2].position;

            return Mathf.Pow(1 - lT, 2) * leftPoint + 2 * (1 - lT) * lT * midPoint + Mathf.Pow(lT, 2) * rightPoint;
        }

        private int GetSection(float t)
        {
            return Mathf.FloorToInt(t);
        }
    }
}