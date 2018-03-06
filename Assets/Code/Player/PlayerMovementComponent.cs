using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;

    [SerializeField] float _moveSpeed = 2.5f;
    [SerializeField] float _rotationTime = 0.05f;


    float _currentRotationVelocity;

    PlayerInputComponent _inputComponent;
    PlayerFlagComponent _flagComponent;

    void Awake()
    {
        _inputComponent = GetComponent<PlayerInputComponent>();
        _flagComponent = GetComponent<PlayerFlagComponent>();
    }

    public void ManualUpdate()
    {
        if (_flagComponent.GetFlag(EFlag.Dead))
            return;

        HandleMovement();
        HandleRotation();
    }


    void HandleRotation()
    {
        float targetAngle = CalculateTargetRotation().eulerAngles.y;

        Vector3 eulerAngles = _rotationTransform.eulerAngles;

        eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, targetAngle, ref _currentRotationVelocity, _rotationTime);

        _rotationTransform.rotation = Quaternion.Euler(eulerAngles);
    }
     
    void HandleMovement()
    {
        Vector3 movement = Vector3.ClampMagnitude(_inputComponent.input.movementDirection * _moveSpeed, _moveSpeed); 

        transform.position += movement * Time.deltaTime;
    }

    Quaternion CalculateTargetRotation()
    {
        return Quaternion.LookRotation(_inputComponent.input.aimTarget - transform.position);
    }
}