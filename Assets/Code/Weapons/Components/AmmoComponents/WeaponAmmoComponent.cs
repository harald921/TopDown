using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WeaponAmmoComponent : MonoBehaviour
{
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

    public virtual void TryReload() { }

    public abstract bool HasAmmo();


    protected void TryInvokeOnReloadStart()
    {
        OnReloadStart?.Invoke();
    }

    protected void TryInvokeOnReloadStop()
    {
        OnReloadStop?.Invoke();
    }
}
