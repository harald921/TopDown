using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionComponent : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] LayerMask _collidesWith;
    [SerializeField] float _environmentCollisionRange = 1.0f;

    Collider _collider;


    void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void ManualUpdate()
    {
        ResolveCollision(CheckCollision(_environmentCollisionRange));
    }

    public Collider[] CheckCollision(float inRange)
    {
        return Physics.OverlapSphere(transform.position - Vector3.up, inRange);
    }

    void ResolveCollision(Collider[] hitColliders)
    {
        foreach (Collider hitCollider in hitColliders)
        {
            if (!_collidesWith.Contains(hitCollider.gameObject.layer))
                continue;

            Vector3 correctionDir;
            float correctionDist;

            // Calculate and perform required movement to resolve collision
            if (Physics.ComputePenetration(_collider, transform.position, transform.rotation, hitCollider, hitCollider.transform.position, hitCollider.transform.rotation, out correctionDir, out correctionDist))
                transform.position += correctionDir * correctionDist;
        }
    }
}