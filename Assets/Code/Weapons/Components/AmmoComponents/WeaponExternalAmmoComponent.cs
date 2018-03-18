using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WeaponExternalAmmoComponent : WeaponAmmoComponent
{
    [SerializeField] float _reloadTime = 1.0f;
    [SerializeField] int _maxAmmo = 30;

    int _currentAmmo;

    WeaponFlagComponent _flagComponent;

    CoroutineHandle _reloadHandle;


    public override void ManualAwake()
    {
        _currentAmmo = _maxAmmo;

        _flagComponent = GetComponent<WeaponFlagComponent>();

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

        _flagComponent.SetFlag(EFlag.Reloading, true);
        yield return Timing.WaitForSeconds(_reloadTime);
        _flagComponent.SetFlag(EFlag.Reloading, false);

        _currentAmmo = _maxAmmo;

        TryInvokeOnReloadFinish();
    }
}
