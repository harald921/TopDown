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
    
    
    void Update()
    {
        CheckAndHandleCollision();
        Move();
        ProgressLifetime();
    }
    
    
    [PunRPC]
    public void SetVelocity(Vector3 inVelocity)
    {
        _velocity = inVelocity;
    }

    void CheckAndHandleCollision()
    {
        RaycastHit[] hits = Physics.SphereCastAll(origin:      transform.position, 
                                                  radius:      _size,
                                                  direction:   _velocity.normalized, 
                                                  maxDistance: _velocity.magnitude * Time.deltaTime);
    
        Debug.Log("TODO: Rather than letting the projectile check where it will be, make it spherecast to its previous position");
    
        foreach (RaycastHit hit in hits)
            if (_collidesWith.Contains(hit.collider.gameObject.layer))
            {
                Debug.Log(hit.collider.name);
                hit.collider.GetComponent<Player>()?.healthComponent.photonView.RPC("DealDamage", PhotonTargets.All, _damage, _type);
                Destroy(gameObject);
            }
    }
    
    void Move()
    {
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
