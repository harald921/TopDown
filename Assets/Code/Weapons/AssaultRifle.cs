using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class AssaultRifle : Weapon
{
    [SerializeField] Transform _muzzleTransform;
    [SerializeField] GameObject _tracerPrefab;
    [SerializeField] LayerMask _collidesWith;
    [SerializeField] Stats _stats;

    int _currentAmmo;


    void Start()
    {
        _currentAmmo = _stats.maxAmmo;
        Timing.RunCoroutine(_IdleState());
    }


    // Internal
    void Fire()
    {
        Raycast()?.GetComponent<PlayerHealthComponent>()?.photonView.RPC("DealDamage", PhotonTargets.All, _stats.damage, _type);


        _currentAmmo--;
    }

    void Reload()
    {
        _currentAmmo = _stats.maxAmmo;
    }

    Collider Raycast()
    {
        List<Vector3> hitPoints = new List<Vector3>();
        hitPoints.Add(_muzzleTransform.position);

        Vector3 projectileDirection = GetProjectileDirection();

        RaycastHit hit;
        if (Physics.Raycast(_muzzleTransform.position, projectileDirection, out hit, _stats.range, _collidesWith))
            hitPoints.Add(hit.point);
        else
            hitPoints.Add(_muzzleTransform.position + (projectileDirection  * _stats.range));

        SpawnTracer(hitPoints);

        return hit.collider;
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

    void SpawnTracer(List<Vector3> inPoints)
    {
        GameObject tracer = PhotonNetwork.Instantiate(_tracerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        tracer.GetComponent<HitscanTracer>().photonView.RPC("_NetInitialize", PhotonTargets.All, inPoints.ToArray());
    }

    IEnumerator<float> _IdleState()
    {
        while (true)
        {
            // Do not proceed unless there's an input component
            while (!_inputComponent)
                yield return Timing.WaitForOneFrame;

            // If the trigger is pulled, shoot
            if (_inputComponent.input.pullWeaponTrigger)
            {
                if (_currentAmmo > 0)
                    yield return Timing.WaitUntilDone(_FireState());
                else
                    yield return Timing.WaitUntilDone(_ReloadState());
            }

            // If the reload button is pressed, reload
            else if (_inputComponent.input.reloadWeapon)
                if (_currentAmmo < _stats.maxAmmo)
                    yield return Timing.WaitUntilDone(_ReloadState());

            yield return Timing.WaitForOneFrame;
        }
    }

    IEnumerator<float> _FireState()
    {
        Fire();
        yield return Timing.WaitForSeconds(_stats.fireTime);
    }

    IEnumerator<float> _ReloadState()
    {
        Reload();
        yield return Timing.WaitForSeconds(_stats.reloadTime);
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
