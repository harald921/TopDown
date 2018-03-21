using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WeaponAmmoComponent : MonoBehaviour
{
    [SerializeField] Stats _stats;
    public Stats stats => _stats;

    protected int _currentAmmo;
    public int currentAmmo => _currentAmmo;
    public bool hasAmmo => _currentAmmo > 0;

    protected Weapon _weapon;

    public event Action OnReloadStart;
    public event Action OnReloadStop;

    protected void TryInvokeOnReloadStart() => OnReloadStart?.Invoke();
    protected void TryInvokeOnReloadStop()
    {
        OnReloadStop?.Invoke();
        Debug.Log("stopping reload");
    }


    public virtual void ManualAwake()
    {
        _weapon = GetComponent<Weapon>();

        _currentAmmo = stats.maxAmmo;
    }

    public virtual void ManualUpdate()
    {
        if (!_weapon.hasOwner)
            return;

        if (_weapon.inputComponent.input.reloadWeapon)
            _weapon.ammoComponent.TryReload();
    }

    public abstract void TryReload();


    [Serializable]
    public struct Stats
    {
        public float reloadTime;
        public int maxAmmo;
    }
}
