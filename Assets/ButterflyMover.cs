using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ButterflyMover : MonoBehaviour
{
    [SerializeField] private float _radiusMin;
    [SerializeField] private float _radiusMax;
    [SerializeField] private Transform _center;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveSpeedVariance;
    [SerializeField] private float _waitTime;
    private Vector3 _from;
    private Vector3 _to;
    private bool _isMoving = false;
    private float _trueMoveSpeed;
    private void Start()
    {
        _isMoving = true;
        _from = transform.position;
        _to = GetNextWaypoint();
        _trueMoveSpeed = Random.Range(_moveSpeed - _moveSpeedVariance / 2, _moveSpeed + _moveSpeedVariance / 2);
    }

    private void Update()
    {
        if (!_isMoving) return;
        Vector3 pos = Vector3.MoveTowards(transform.position, _to, _trueMoveSpeed);
        transform.position = pos;
        if (Vector3.Distance(transform.position, _to) < 0.05f)
        {
            _from = _to;
            _to = GetNextWaypoint();
            _trueMoveSpeed = Random.Range(_moveSpeed - _moveSpeedVariance / 2, _moveSpeed + _moveSpeedVariance / 2);
            StartCoroutine(Wait(_waitTime));
        }
    }

    private Vector3 GetNextWaypoint()
    {
        Vector3 relPos = Random.insideUnitSphere;
        while (relPos.y < 0.01f)
        {
            relPos = Random.insideUnitSphere;
        }

        float radius = Random.Range(_radiusMin, _radiusMax);
        Vector3 truePos = relPos * radius + _center.position;
        return truePos;
    }

    private IEnumerator Wait(float s)
    {
        _isMoving = false;
        yield return new WaitForSeconds(s);
        _isMoving = true;
    }
}
