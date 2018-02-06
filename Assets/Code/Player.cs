using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;

    [Header("Movement")]
    [SerializeField] float _moveSpeed = 2.5f;

    [Header("Rotation")]
    [SerializeField] float _rotationSpeed = 10.0f;

    [Header("Collision")]
    [SerializeField] float _environmentCollisionRange = 1.0f;

    Rigidbody _rigidBody;
    Camera _mainCamera;
    Collider _collider;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();    
        _mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }


    void HandleRotation()
    {
        _rotationTransform.rotation = Quaternion.RotateTowards(_rotationTransform.rotation, CalculateTargetRotation(), _rotationSpeed * Time.deltaTime);
    }

    Quaternion CalculateTargetRotation()
    {
        Vector3 aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.localScale.y, aimTarget.z);

        return Quaternion.LookRotation(aimTarget - transform.position);
    }


    void HandleMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveSpeed * Time.deltaTime;
        transform.position += movement;

        ResolveCollision(CheckCollision());
    }

    Collider[] CheckCollision()
    {
        return Physics.OverlapSphere(transform.position - Vector3.up * 1, _environmentCollisionRange);
    }

    void ResolveCollision(Collider[] hitColliders)
    {
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider == _collider)
                continue;

            Vector3 otherPos = hitCollider.transform.position;
            Quaternion otherRot = hitCollider.transform.rotation;

            Vector3 correctionDir;
            float correctionDist;

            if (Physics.ComputePenetration(_collider, transform.position, transform.rotation, hitCollider, hitCollider.transform.position, hitCollider.transform.rotation, out correctionDir, out correctionDist))
            {
                transform.position += correctionDir * correctionDist;
            }
        }
    }


}
