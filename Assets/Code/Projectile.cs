using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _size = 0.1f;
    [SerializeField] LayerMask _layersThatStop;

    Vector3 _velocity;
    float _lifetime;

    Rigidbody _rigidbody;


    public void Initialize(float inLifetime, Vector3 inVelocity)
    {
        _lifetime = inLifetime;
        _velocity = inVelocity;
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();    
    }

    void Update()
    {
        Move();
        CheckCollision();
        ProgressLifetime();
    }

    void ProgressLifetime()
    {
        _lifetime -= Time.deltaTime;

        if (_lifetime <= 0)
            Destroy(gameObject);
    }

    void Move()
    {
        transform.position += _velocity * Time.deltaTime;
    }

    void CheckCollision()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _size, _velocity.normalized, _velocity.magnitude * Time.deltaTime);
        foreach (RaycastHit hit in hits)
        {
            if (_layersThatStop.Contains(hit.collider.gameObject.layer))
                OnCollision(hit.collider);
        }
    }

    void OnCollision(Collider hitCollider)
    {
        Destroy(gameObject);
    }
}
