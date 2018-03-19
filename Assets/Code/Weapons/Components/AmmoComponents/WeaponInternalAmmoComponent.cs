using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WeaponInternalAmmoComponent : WeaponAmmoComponent
{
    [SerializeField] float _reloadTime = 1.0f;
    [SerializeField] int _maxAmmo = 30;

    int _currentAmmo;

    CoroutineHandle _reloadHandle;

    Weapon _weapon;


    public override void ManualAwake()
    {
        _weapon = GetComponent<Weapon>();

        _currentAmmo = _maxAmmo;

        _weapon.OnDropped += CancelReload;
        _weapon.fireComponent.OnFire += () => _currentAmmo--;

        base.ManualAwake();
    }

    public override bool HasAmmo()
    {
        return _currentAmmo > 0;
    }

    public override void TryReload()
    {
        if (_currentAmmo < _maxAmmo)
            _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
    }

    IEnumerator<float> _HandleReload()
    {
        TryInvokeOnReloadStart();

        while (_currentAmmo < _maxAmmo)
        {
            yield return Timing.WaitForSeconds(_reloadTime);
            _currentAmmo++;
        }



        TryInvokeOnReloadStop();
    }

    void CancelReload()
    {
        Timing.KillCoroutines(_reloadHandle);
        TryInvokeOnReloadStop();
    }
}
