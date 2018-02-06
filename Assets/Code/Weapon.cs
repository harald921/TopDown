using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Sounds
    {
        public AudioClip fire;
        public AudioClip reload;
    }
    [SerializeField] Sounds _sounds;

    public enum FiringMechanism
    {
        Auto,
        Semi
    }
    [SerializeField] FiringMechanism _firingMechanism;

    [System.Serializable]
    public struct Stats
    {
        public float fireRate;
        public float projectileSpread;
        public float velocityInconsistensy;

        public float damageMin;
        public float damageMax;

        public int projectilesPerShot;

        public float projectileSpeed;

        public float projectileLifeTime;

        public int maxAmmo;
        public float reloadSpeed;
    }
    [SerializeField] Stats _stats;

    [SerializeField] GameObject _projectileGO;

    Transform _muzzleTransform;

    bool _triggerReleased = true;

    bool  _isFiring       = false;
    float _fireProgress   = 1.0f;
    bool  _isReloading    = false;
    float _reloadProgress = 1.0f;

    int _weaponAmmoCurrent;


    void Start()
    {
        _muzzleTransform = transform.GetChild(0);

        _weaponAmmoCurrent = _stats.maxAmmo;
    }

    void Update()
    {
        ProgressFireTimer();
        ProgressReloadTimer();
    }
    
    void ProgressFireTimer()
    {
        // Add time to the fire progress and check if it is finished
        if (_isFiring)
            if ((_fireProgress += _stats.fireRate * Time.deltaTime) >= 1)
            {
                _isFiring = false;
                _fireProgress = 1;
            }
    }

    void ProgressReloadTimer()
    {
        // Add time to the reload progress and check if it is finished
        if (_isReloading)
            if ((_reloadProgress += _stats.reloadSpeed * Time.deltaTime) >= 1)
            {
                // Refill ammo in weapon
                _weaponAmmoCurrent = _stats.maxAmmo;

                _isReloading = false;
                _reloadProgress = 1;
            }
    }

    /* External Methods */
    public void PullTrigger()
    {
        // Return if weapon is reloading or firing
        if (_isFiring || _isReloading)
            return;

        if (_firingMechanism == FiringMechanism.Semi)
            if (!_triggerReleased)
                return;

        // Reload if the weapon has no ammo
        if (_weaponAmmoCurrent == 0)
        {
            TryReload();
            return;
        }

        _triggerReleased = false;

        Fire();
    }

    public void ReleaseTrigger()
    {
        _triggerReleased = true;
    }

    void Fire()
    {
        for (int i = 0; i < _stats.projectilesPerShot; i++)
            CreateProjectile();

        _weaponAmmoCurrent--;

        // PlayGunshotSound();

        _isFiring = true;
        _fireProgress = 0;
    }

    void CreateProjectile()
    {
        GameObject newProjectileGO = Instantiate(_projectileGO);
        newProjectileGO.transform.position = _muzzleTransform.position;

        // Calculate new projectiles vector
        Vector3 newProjectileSpread = new Vector3(Random.Range(-_stats.projectileSpread, _stats.projectileSpread), 0, Random.Range(-_stats.projectileSpread, _stats.projectileSpread));
        Vector3 newProjectileDirection = _muzzleTransform.forward + newProjectileSpread;
        float newProjectileVelocityModifier = _stats.projectileSpeed + Random.Range(-_stats.velocityInconsistensy, _stats.velocityInconsistensy);

        newProjectileGO.GetComponent<Projectile>().Initialize(_stats.projectileLifeTime, newProjectileDirection.normalized * newProjectileVelocityModifier);
    }

    void PlayGunshotSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.PlayOneShot(_sounds.fire);
    }

    public void TryReload()
    {
        // Return if the ammo is already reloaded
        if (_weaponAmmoCurrent == _stats.maxAmmo)
            return;

        // Return if the weapon is already being reloaded
        if (_isReloading)
            return;

        // PlayReloadSound();

        _isReloading = true;
        _reloadProgress = 0;
    }

    void PlayReloadSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.PlayOneShot(_sounds.reload);
    }
}
