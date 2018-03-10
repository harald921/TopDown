using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class HitscanMagazineWeapon : Weapon
{
    [SerializeField] GameObject _tracerPrefab;
    [SerializeField] LayerMask  _collidesWith;
    [SerializeField] Stats      _stats;
    [SerializeField] bool       _semiAuto;

    int _currentAmmo;

    CoroutineHandle _fireHandle;
    CoroutineHandle _reloadHandle;


    void Awake()
    {
        _currentAmmo = _stats.maxAmmo;
    }

    void Update()
    {
        if (!_inputComponent)
            return;

        if (_inputComponent.input.weaponTriggerPulled)
        {
            if (_currentAmmo > 0 && !_reloadHandle.IsRunning)
                _fireHandle = Timing.RunCoroutineSingleton(_HandleFire(), _fireHandle, SingletonBehavior.Abort);
            else
                _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
        }

        if (_inputComponent.input.reloadWeapon)
            _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
    }


    public override void PickUp(PlayerInputComponent inInputComponent)
    {
        _inputComponent = inInputComponent;
    }

    public override void Drop()
    {
        Timing.KillCoroutines(_reloadHandle);
        _inputComponent = null;
    }


    IEnumerator<float> _HandleFire()
    {
        Fire();

        while (_semiAuto && _inputComponent.input.weaponTriggerPulled)
            yield return Timing.WaitForOneFrame;

        yield return Timing.WaitForSeconds(_stats.fireTime);
    }

    IEnumerator<float> _HandleReload()
    {
        yield return Timing.WaitForSeconds(_stats.reloadTime);
        Reload();
    }


    void Reload()
    {
        _currentAmmo = _stats.maxAmmo;
    }

    void Fire()
    {
        Collider hitCollider = MuzzleOverlapSphere();

        if (!hitCollider)
        {
            List<Vector3> hitPoints;
            hitCollider = HitScan(out hitPoints);
            SpawnTracer(hitPoints);
        }

        hitCollider?.GetComponent<PlayerHealthComponent>()?.photonView.RPC("DealDamage", PhotonTargets.All, _stats.damage, _type);

        TryInvokeOnFire();

        _currentAmmo--;
    }

    Collider HitScan(out List<Vector3> outHitPoints)
    {
        Vector3 projectileDirection = GetProjectileDirection();

        outHitPoints = new List<Vector3>() {
            _muzzleTransform.position
        };

        RaycastHit hit;
        if (Physics.Raycast(_muzzleTransform.position, projectileDirection, out hit, _stats.range, _collidesWith))
            outHitPoints.Add(hit.point);
        else
            outHitPoints.Add(_muzzleTransform.position + (projectileDirection * _stats.range));

        return hit.collider;
    }

    Collider MuzzleOverlapSphere()
    {
        Collider[] collidersCoveringMuzzle = Physics.OverlapSphere(_muzzleTransform.position, float.Epsilon, _collidesWith);
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

    Vector3 GetProjectileDirection()
    {
        Vector3 projectileSpread = new Vector3()
        {
            x = Random.Range(-_stats.spread, _stats.spread),
            z = Random.Range(-_stats.spread, _stats.spread)
        };

        return _muzzleTransform.forward + projectileSpread;
    }
   

    [System.Serializable]
    public struct Stats
    {
        [Space(5)]
        public int   damage;
        public float fireTime;
        public float spread;
        public float range;

        [Space(5)]
        public int   maxAmmo;
        public float reloadTime;
    }
}
