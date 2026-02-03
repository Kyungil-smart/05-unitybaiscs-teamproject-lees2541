using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class KTS_CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset;
    // [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private Transform target;
    private Transform cameraTransform;
    
 
    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }


    private void Update()
    {
        
    }    
    
    private void LateUpdate()
    {
        cameraTransform.position = target.position + cameraOffset;
        
    }
}
