using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Projectile : Photon.MonoBehaviour
{
    [SerializeField] float       _size;
    [SerializeField] int         _damage;
    [SerializeField] float       _lifetime;
    [SerializeField] Weapon.Type _type;
    [SerializeField] LayerMask   _collidesWith;
    
    Vector3 _velocity;
    Vector3 _previousPosition;


    void Update()
    {
        Move();
        CheckAndHandleCollision();
        ProgressLifetime();
    }
    
    
    [PunRPC]
    public void SetVelocity(Vector3 inVelocity)
    {
        _velocity = inVelocity;
    }

    void CheckAndHandleCollision()
    {
        Vector3 toPreviousPosition = transform.position - _previousPosition;

        RaycastHit[] hits = Physics.SphereCastAll(origin:      transform.position, 
                                                  radius:      _size,
                                                  direction:   toPreviousPosition.normalized, 
                                                  maxDistance: toPreviousPosition.magnitude);
    
        foreach (RaycastHit hit in hits)
            if (_collidesWith.Contains(hit.collider.gameObject.layer))
            {
                hit.collider.GetComponent<Player>()?.healthComponent.photonView.RPC("DealDamage", PhotonTargets.All, _damage, _type);
                Destroy(gameObject);
            }
    }
    
    void Move()
    {
        _previousPosition = transform.position;
        transform.position += _velocity * Time.deltaTime;
    }
    
    void ProgressLifetime()
    {
        _lifetime -= Time.deltaTime;
        
        if (_lifetime <= 0)
        {
            Debug.Log(_lifetime);
            Destroy(gameObject);
        }
    }
}
