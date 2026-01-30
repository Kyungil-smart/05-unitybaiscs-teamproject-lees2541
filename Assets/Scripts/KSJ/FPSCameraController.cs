using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    [SerializeField] private Transform _viewPoint;
    
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _pitchMin;
    [SerializeField] private float _pitchMax;

    private float _pitch;

    private void Update()
    {
        Rotation();
    }

    private void Rotation()
    {
        float x = Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float y = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        
        transform.Rotate(Vector3.up,x);

        _pitch -= y;
        _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
        
        _viewPoint.localRotation = Quaternion.Euler(_pitch,0,0);
        
    }
    
}
