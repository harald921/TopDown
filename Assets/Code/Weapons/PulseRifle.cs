using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

// Projectile
// Magazine

public class PulseRifle : Weapon
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] SpecificStats _specificStats;

    CoroutineHandle _fireHandle;
    CoroutineHandle _reloadHandle;

    void Awake()
    {
        _currentAmmo = stats.maxAmmo;
    }

    void Update()
    {
        if (!_inputComponent)
            return;

        if (_inputComponent.input.weaponTriggerPulled)
        {
            if (_currentAmmo > 0)
            {
                if (!_reloadHandle.IsRunning)
                    _fireHandle = Timing.RunCoroutineSingleton(_HandleFire(), _fireHandle, SingletonBehavior.Abort);
            }

            else
                _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
        }
    }

    public override void Drop()
    {
        Timing.KillCoroutines(_reloadHandle);

        base.Drop();
    }

    IEnumerator<float> _HandleFire()
    {
        Fire();
        yield return Timing.WaitForSeconds(stats.fireTime);
    }

    IEnumerator<float> _HandleReload()
    {
        TryInvokeReloadStart();
        yield return Timing.WaitForSeconds(stats.reloadTime);
        Reload();
    }

    /// <summary>
    /// Reloads
    /// </summary>
    void Reload()
    {
        _currentAmmo = stats.maxAmmo;
        TryInvokeReloadFinish();
    }

    void Fire()
    {
        GameObject newProjectile = PhotonNetwork.Instantiate(_projectilePrefab.name, _muzzleTransform.position, Quaternion.identity, 0);
        newProjectile.GetPhotonView().RPC("SetVelocity", PhotonTargets.All, GetProjectileDirection() * _specificStats.projectileSpeed);

        _currentAmmo--;

        TryInvokeOnFire();
    }

    Vector3 GetProjectileDirection()
    {
        Vector3 projectileSpread = new Vector3()
        {
            x = Random.Range(-stats.spread, stats.spread),
            z = Random.Range(-stats.spread, stats.spread)
        };

        return _muzzleTransform.forward + projectileSpread;
    }

    [System.Serializable]
    public struct SpecificStats
    {
        public float projectileSpeed;
    }
}
