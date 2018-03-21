using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WeaponExternalAmmoComponent : WeaponAmmoComponent
{
    CoroutineHandle _reloadHandle;

    public override void ManualAwake()
    {
        base.ManualAwake();

        _currentAmmo = stats.maxAmmo;

        _weapon.OnDropped += CancelReload;
        _weapon.fireComponent.OnFire += () => _currentAmmo--;
    }

    public override void TryReload()
    {
        if (_currentAmmo < stats.maxAmmo)
            _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
    }

    IEnumerator<float> _HandleReload()
    {
        TryInvokeOnReloadStart();

        yield return Timing.WaitForSeconds(stats.reloadTime);

        _currentAmmo = stats.maxAmmo;

        TryInvokeOnReloadStop();
    }

    void CancelReload()
    {
        Timing.KillCoroutines(_reloadHandle);
        TryInvokeOnReloadStop();
    }
}
