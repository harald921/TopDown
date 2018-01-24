using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Exposed data
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 2.5f;

    // References
    Rigidbody _rigidBody;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();    
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
         _rigidBody.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveSpeed;
    }
    
    void HandleRotation()
    {

    }
}
