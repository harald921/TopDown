using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WeaponExternalAmmoComponent : WeaponAmmoComponent
{
    [SerializeField] Stats _stats;
    public Stats stats => _stats;

    int _currentAmmo;
    public int currentAmmo => _currentAmmo;

    CoroutineHandle _reloadHandle;



    public override void ManualAwake()
    {
        base.ManualAwake();

        _currentAmmo = _stats.maxAmmo;

        _weapon.OnDropped += CancelReload;
        _weapon.fireComponent.OnFire += () => _currentAmmo--;
    }

    public override bool HasAmmo()
    {
        return _currentAmmo > 0;
    }

    public override void TryReload()
    {
        if (_currentAmmo < _stats.maxAmmo)
            _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
    }

    IEnumerator<float> _HandleReload()
    {
        TryInvokeOnReloadStart();

        yield return Timing.WaitForSeconds(_stats.reloadTime);

        _currentAmmo = _stats.maxAmmo;

        TryInvokeOnReloadStop();
    }

    void CancelReload()
    {
        Timing.KillCoroutines(_reloadHandle);
        TryInvokeOnReloadStop();
    }

    [System.Serializable]
    public struct Stats
    {
        public float reloadTime;
        public int   maxAmmo;
    }
}
