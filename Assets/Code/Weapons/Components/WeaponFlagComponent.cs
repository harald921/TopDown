using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class WeaponFlagComponent : Photon.MonoBehaviour
{
    // Private
    Dictionary<EWeaponFlag, bool> _flags = new Dictionary<EWeaponFlag, bool>();
    Dictionary<EWeaponFlag, CoroutineHandle> _flagHandles = new Dictionary<EWeaponFlag, CoroutineHandle>();


    public void ManualAwake()
    {
        foreach (EWeaponFlag flag in Enum.GetValues(typeof(EWeaponFlag)))
        {
            _flags.Add(flag, false);
            _flagHandles.Add(flag, new CoroutineHandle());
        }

        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        Weapon weapon = GetComponent<Weapon>();

        weapon.ammoComponent.OnReloadStart += () => SetFlag(EWeaponFlag.Reloading, true);
        weapon.ammoComponent.OnReloadStop  += () => SetFlag(EWeaponFlag.Reloading, false);
    }

    // External
    public void SetFlag(EWeaponFlag inFlag, bool inState, float inDuration = 0.0f, bool inNetTransfer = false)
    {
        if (inNetTransfer)
            photonView.RPC("NetSetFlag", PhotonTargets.All, inFlag, inState, inDuration);
        else
            NetSetFlag(inFlag, inState, inDuration);
    }

    public bool GetFlag(EWeaponFlag inFlag)
    {
        return _flags[inFlag];
    }


    // Internal
    [PunRPC]
    void NetSetFlag(EWeaponFlag inFlag, bool inState, float inDuration = 0.0f)
    {
        _flags[inFlag] = inState;

        // Start a duration coroutine, and overwrite any earlier duration coroutine of the same type
        if (inDuration > 0)
            _flagHandles[inFlag] = Timing.RunCoroutineSingleton(HandleDuration(inFlag, inDuration, inState), _flagHandles[inFlag], SingletonBehavior.Overwrite);

        // If the flag is set without a duration and a duration coroutine is going, kill the coroutine
        else
            Timing.KillCoroutines(_flagHandles[inFlag]);
    }

    IEnumerator<float> HandleDuration(EWeaponFlag inFlag, float inDuration, bool inState)
    {
        yield return Timing.WaitForSeconds(inDuration);
        SetFlag(inFlag, !inState);
    }
}

public enum EWeaponFlag
{
    Reloading,
    WaitingForTriggerRelease
}
