using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WeaponInternalAmmoComponent : WeaponAmmoComponent
{
    CoroutineHandle _reloadHandle;
    CoroutineHandle _manualCancelHandle;

    public override void ManualAwake()
    {
        base.ManualAwake();

        _weapon.OnDropped += StopReload;
        _weapon.fireComponent.OnFire += () => _currentAmmo--;
    }

    public override void ManualUpdate()
    {
        if (!_weapon.hasOwner)
            return;

        base.ManualUpdate();

        CheckForReloadCancel();
    }


    public override void TryReload()
    {
        if (_currentAmmo < stats.maxAmmo)
            _reloadHandle = Timing.RunCoroutineSingleton(_HandleReload(), _reloadHandle, SingletonBehavior.Abort);
    }

    IEnumerator<float> _HandleReload()
    {
        while (_currentAmmo < stats.maxAmmo)
        {
            TryInvokeOnReloadStart();
            yield return Timing.WaitForSeconds(stats.reloadTime);
            _currentAmmo++;
            TryInvokeOnReloadStop();
        }
    }

    void CheckForReloadCancel()
    {
        bool triggerPulled = _weapon.inputComponent.input.weaponTriggerPulled;
        bool waitingForTriggerRelease = _weapon.flagComponent.GetFlag(EWeaponFlag.WaitingForTriggerRelease);

        if (triggerPulled && hasAmmo && !waitingForTriggerRelease)
            if (_reloadHandle.IsRunning)
                StopReload();
    }


    void StopReload()
    {
        Timing.KillCoroutines(_reloadHandle);
        TryInvokeOnReloadStop();
    }
}
