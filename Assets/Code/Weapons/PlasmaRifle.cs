using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaRifle : MonoBehaviour
{
    // [SerializeField] GameObject _projectilePrefab;
    // 
    // void Fire()
    // {
    //     for (int i = 0; i < _stats.projectilesPerShot; i++)
    //         CreateProjectile();
    // 
    //     _currentAmmo--;
    // }
    // 
    // 
    // void CreateProjectile()
    // {
    //     GameObject newProjectileGO = PhotonNetwork.Instantiate(_projectilePrefab.name, _muzzleTransform.position, Quaternion.identity, 0);
    // 
    //     // Calculate new projectiles vector
    //     Vector3 newProjectileSpread = new Vector3(Random.Range(-_stats.spread, _stats.spread), 0, Random.Range(-_stats.spread, _stats.spread));
    //     Vector3 newProjectileDirection = _muzzleTransform.forward + newProjectileSpread;
    //     float newProjectileVelocityModifier = _stats.velocity + Random.Range(-_stats.velocityInconsistency, _stats.velocityInconsistency);
    // 
    //     newProjectileGO.GetComponent<Projectile>().Initialize(_stats.damage, _stats.projectileLifeTime, newProjectileDirection.normalized * newProjectileVelocityModifier);
    // }
}
