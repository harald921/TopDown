using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Weapon : Photon.MonoBehaviour
{
    [SerializeField] FiringMechanism _firingMechanism;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Stats _stats;
    public Stats stats => _stats;

    int _currentAmmo     = 0;

    Transform _muzzleTransform;

    PlayerInputComponent _inputComponent;


    void Start()
    {
        _muzzleTransform = transform.GetChild(0);

        _currentAmmo = _stats.maxAmmo;

        Timing.RunCoroutine(_IdleState());
    }


    // External
    public void PickUp(Player inPlayer)
    {
        _inputComponent = inPlayer.inputComponent;
    }

    public void Drop()
    {
        _inputComponent = null;
    }


    // Internal
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
        // Fire and wait...
        Fire();
        yield return Timing.WaitForSeconds(_stats.fireTime);

        // If it's a semi auto, wait until the trigger is released
        if (_firingMechanism == FiringMechanism.Semi)
            while (_inputComponent.input.pullWeaponTrigger)
                yield return Timing.WaitForOneFrame;
    }

    IEnumerator<float> _ReloadState()
    {
        Reload();
        yield return Timing.WaitForSeconds(_stats.reloadTime);
    }


    void Fire()
    {
        for (int i = 0; i < _stats.projectilesPerShot; i++)
            CreateProjectile();

        _currentAmmo--;
    }

    void Reload()
    {
        _currentAmmo = _stats.maxAmmo;
    }

    void CreateProjectile()
    {
        GameObject newProjectileGO = PhotonNetwork.Instantiate(_projectilePrefab.name, _muzzleTransform.position, Quaternion.identity, 0);

        // Calculate new projectiles vector
        Vector3 newProjectileSpread = new Vector3(Random.Range(-_stats.spread, _stats.spread), 0, Random.Range(-_stats.spread, _stats.spread));
        Vector3 newProjectileDirection = _muzzleTransform.forward + newProjectileSpread;
        float newProjectileVelocityModifier = _stats.velocity + Random.Range(-_stats.velocityInconsistency, _stats.velocityInconsistency);

        newProjectileGO.GetComponent<Projectile>().Initialize(_stats.damage, _stats.projectileLifeTime, newProjectileDirection.normalized * newProjectileVelocityModifier);
    }






    [System.Serializable]
    public struct Stats
    {
        [Space(5)]
        public int damage;

        [Space(5)]
        public float fireTime;
        public int   projectilesPerShot;
        public float projectileLifeTime;

        [Space(5)]
        public float spread;
        public float velocity;
        public float velocityInconsistency;

        [Space(5)]
        public int maxAmmo;
        public float reloadTime;
    }

    public enum FiringMechanism
    {
        Auto,
        Semi
    }
}
