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


    public virtual void ManualAwake()
    {
        _weapon = GetComponent<Weapon>();
    }

    public void ManualUpdate()
    {
        if (!_weapon.hasOwner)
            return;

        if (_weapon.inputComponent.input.reloadWeapon)
            _weapon.ammoComponent.TryReload();
    }

    public abstract void TryReload();


    protected void TryInvokeOnReloadStart()
    {
        OnReloadStart?.Invoke();
    }

    protected void TryInvokeOnReloadStop()
    {
        OnReloadStop?.Invoke();
    }


    [Serializable]
    public struct Stats
    {
        public float reloadTime;
        public int maxAmmo;
    }
}
