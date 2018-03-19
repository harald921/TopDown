using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileFireComponent : WeaponFireComponent
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] SpecificStats _specificStats;


    protected override void Fire()
    {
        GameObject newProjectile = PhotonNetwork.Instantiate(_projectilePrefab.name, _weapon.muzzleTransform.position, Quaternion.identity, 0);
        newProjectile.GetPhotonView().RPC("SetVelocity", PhotonTargets.All, GetProjectileDirection() * _specificStats.projectileSpeed);

        TryInvokeOnFire();
    }


    [System.Serializable]
    public struct SpecificStats
    {
        public float projectileSpeed;
    }
}
