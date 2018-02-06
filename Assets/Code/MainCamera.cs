using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] Vector3 _cameraOffset = new Vector3(0.0f, 15.0f, -5.0f);
    [SerializeField] float _smoothTime = 0.1f;

    Vector3 targetPosition { get { return _targetTransform.position + _cameraOffset; } }

    Vector3 _velocity;


    private void Awake()
    {
        transform.position = targetPosition;   
    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);        
    }
}
