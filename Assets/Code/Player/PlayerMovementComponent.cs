using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;

    [SerializeField] float _moveSpeed = 2.5f;
    [SerializeField] float _rotationSpeed = 10.0f;


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
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    Quaternion CalculateTargetRotation()
    {
        return Quaternion.LookRotation(GetAimTarget() - transform.position);
    }

    Vector3 GetAimTarget()
    {
        Vector3 aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.localScale.y, aimTarget.z);

        return aimTarget;
    }
}