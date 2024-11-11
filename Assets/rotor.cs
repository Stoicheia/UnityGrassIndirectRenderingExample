using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotor : MonoBehaviour
{
    [SerializeField] private float _rotSpeed;

    private float _currentRotation;
    // Start is called before the first frame update
    void Start()
    {
        _currentRotation = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        _currentRotation += _rotSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0,_currentRotation,0);
    }
}
