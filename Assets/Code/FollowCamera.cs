using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    static Transform _targetTransform;

    [SerializeField] Vector3 _cameraOffset = new Vector3(0.0f, 15.0f, -5.0f);
    [SerializeField] float _smoothTime = 0.1f;

    Vector3 targetPosition { get { return _targetTransform.position + _cameraOffset; } }

    Vector3 _velocity;

    void LateUpdate()
    {
        if (!_targetTransform)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);        
    }

    public static void SetTarget(Transform inTarget)
    {
        _targetTransform = inTarget;    
    }
}
