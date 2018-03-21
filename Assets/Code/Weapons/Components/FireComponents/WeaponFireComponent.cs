using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public abstract class WeaponFireComponent : MonoBehaviour
{
    [SerializeField] protected Stats _stats;
    public Stats stats => _stats;

    protected Weapon _weapon;

    CoroutineHandle _fireHandle;

    public event Action OnFire;


    public virtual void ManualAwake()
    {
        _weapon = GetComponent<Weapon>();
    }

    public void ManualUpdate()
    {
        if (!_weapon.hasOwner)
            return;

        bool triggerPulled = _weapon.inputComponent.input.weaponTriggerPulled;
        bool isFiring      = _weapon.flagComponent.GetFlag(EWeaponFlag.Firing);

        if (triggerPulled && !isFiring)
        {
            bool isReloading = _weapon.flagComponent.GetFlag(EWeaponFlag.Reloading);
            if (isReloading)
                return;

            if (_weapon.ammoComponent.hasAmmo)
                _fireHandle = Timing.RunCoroutineSingleton(_HandleFire(), _fireHandle, SingletonBehavior.Abort);

            else
            {
                _weapon.ammoComponent.TryReload();
            }
        }
    }

    IEnumerator<float> _HandleFire()
    {
        Fire();

        yield return Timing.WaitForSeconds(_stats.fireTime);

        while (_stats.fireMode == FireMode.Semi && _weapon.inputComponent.input.weaponTriggerPulled)
            yield return Timing.WaitForOneFrame;
    }

    protected abstract void Fire();

    protected void TryInvokeOnFire()
    {
        OnFire?.Invoke();
    }

    protected Vector3 GetProjectileDirection()
    {
        Vector3 projectileSpread = new Vector3()
        {
            x = UnityEngine.Random.Range(-_stats.spread, _stats.spread),
            z = UnityEngine.Random.Range(-_stats.spread, _stats.spread)
        };

        return _weapon.muzzleTransform.forward + projectileSpread;
    }

    [Serializable]
    public struct Stats
    {
        public FireMode fireMode;
        public float fireTime;
        public float spread;
        public float recoilMoveSlow;
    }

    public enum FireMode
    {
        Semi,
        Burst,
        Auto
    }
}