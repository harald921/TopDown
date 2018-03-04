using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;

    [SerializeField] float _moveSpeed = 2.5f;
    [SerializeField] float _rotationSpeed = 10.0f;

    PlayerInputComponent _inputComponent;


    void Awake()
    {
        _inputComponent = GetComponent<PlayerInputComponent>();
    }

    public void ManualUpdate()
    {
        HandleMovement();
        HandleRotation();
    }


    void HandleRotation()
    {
        _rotationTransform.rotation = Quaternion.RotateTowards(_rotationTransform.rotation, CalculateTargetRotation(), _rotationSpeed * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 movement = _inputComponent.input.movementDirection * _moveSpeed; 
        transform.position += movement * Time.deltaTime;
    }

    Quaternion CalculateTargetRotation()
    {
        return Quaternion.LookRotation(_inputComponent.input.aimTarget - transform.position);
    }
}