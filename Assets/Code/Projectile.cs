using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Photon.MonoBehaviour
{
    [SerializeField] float _size = 0.1f;
    [SerializeField] LayerMask _layersThatStop;

    Vector3 _velocity;
    float _lifetime;

    Rigidbody _rigidbody;


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();    
    }

    public void Initialize(float inLifetime, Vector3 inVelocity)
    {
        _lifetime = inLifetime;
        _velocity = inVelocity;

        photonView.RPC("NetInitialize", PhotonTargets.Others, inLifetime, inVelocity, transform.position);
    }

    [PunRPC]
    void NetInitialize(float inLifetime, Vector3 inVelicity, Vector3 inOrigin, PhotonMessageInfo inInfo)
    {
        float netDelta = NetworkManager.CalculateNetDelta(inInfo.timestamp) * 3;

        _lifetime = inLifetime - netDelta;         
        _velocity = inVelicity;
        transform.position = inOrigin + (_velocity * netDelta);

        CheckCollision(inOrigin, netDelta);
    }

    void Update()
    {
        Move();
        CheckCollision(transform.position);
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

    void CheckCollision(Vector3 inOrigin, float inNetDelta = 0)
    {
        RaycastHit[] hits = Physics.SphereCastAll(inOrigin, _size, _velocity.normalized, _velocity.magnitude * (Time.deltaTime + inNetDelta));
        foreach (RaycastHit hit in hits)
            if (_layersThatStop.Contains(hit.collider.gameObject.layer))
                OnCollision(hit.collider);
    }

    void OnCollision(Collider hitCollider)
    {
        Player hitPlayer = hitCollider.GetComponent<Player>();
        if (hitPlayer)
        {
            hitPlayer.photonView.RPC("ModifyHealth", PhotonTargets.All, -10);
            Debug.Log("Temporary debug damage");
        }

        Destroy(gameObject);
    }
}
