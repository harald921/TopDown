using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WeaponHitscanFireComponent : WeaponFireComponent
{
    [SerializeField] GameObject      _tracerPrefab;
    [SerializeField] LayerMask       _collidesWith;
    [SerializeField] ProjectileStats _projectileStats;


    protected override void Fire()
    {
        Collider hitCollider = MuzzleOverlapSphere();

        if (!hitCollider)
        {
            List<Vector3> hitPoints;
            hitCollider = HitScan(out hitPoints);
            SpawnTracer(hitPoints);
        }

        hitCollider?.GetComponent<PlayerHealthComponent>()?.photonView.RPC("DealDamage", PhotonTargets.All, _projectileStats.damage, _projectileStats._type);

        TryInvokeOnFire();
    }


    Collider HitScan(out List<Vector3> outHitPoints)
    {
        Vector3 projectileDirection = GetProjectileDirection();

        outHitPoints = new List<Vector3>() {
            _weapon.muzzleTransform.position
        };

        RaycastHit hit;
        if (Physics.Raycast(_weapon.muzzleTransform.position, projectileDirection, out hit, _projectileStats.range, _collidesWith))
            outHitPoints.Add(hit.point);
        else
            outHitPoints.Add(_weapon.muzzleTransform.position + (projectileDirection * _projectileStats.range));

        return hit.collider;
    }

    Collider MuzzleOverlapSphere()
    {
        Collider[] collidersCoveringMuzzle = Physics.OverlapSphere(_weapon.muzzleTransform.position, float.Epsilon, _collidesWith);
        if (collidersCoveringMuzzle.Length > 0)
            return collidersCoveringMuzzle[0];
        else
            return null;
    }

    void SpawnTracer(List<Vector3> inPoints)
    {
        GameObject tracer = PhotonNetwork.Instantiate(_tracerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        tracer.GetComponent<HitscanTracer>().photonView.RPC("_NetInitialize", PhotonTargets.All, inPoints.ToArray());
    }

    
    [System.Serializable]
    public struct ProjectileStats
    {
        public Weapon.Type _type;
        public int damage;
        public float range;
    }
}
