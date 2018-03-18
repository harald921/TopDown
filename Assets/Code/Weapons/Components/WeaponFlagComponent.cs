using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class WeaponFlagComponent : Photon.MonoBehaviour
{
    // Private
    Dictionary<EFlag, bool> _flags = new Dictionary<EFlag, bool>();
    Dictionary<EFlag, CoroutineHandle> _flagHandles = new Dictionary<EFlag, CoroutineHandle>();


    public void ManualAwake()
    {
        foreach (EFlag flag in Enum.GetValues(typeof(EFlag)))
        {
            _flags.Add(flag, false);
            _flagHandles.Add(flag, new CoroutineHandle());
        }

        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        Player player = GetComponent<Player>();
        player.healthComponent.OnDeath += () => SetFlag(EFlag.Dead, true);
        player.respawnComponent.OnSpawn += () => SetFlag(EFlag.Dead, false);
    }

    // External
    public void SetFlag(EFlag inFlag, bool inState, float inDuration = 0.0f, bool inNetTransfer = false)
    {
        if (inNetTransfer)
            photonView.RPC("NetSetFlag", PhotonTargets.All, inFlag, inState, inDuration);
        else
            NetSetFlag(inFlag, inState, inDuration);
    }

    public bool GetFlag(EFlag inFlag)
    {
        return _flags[inFlag];
    }


    // Internal
    [PunRPC]
    void NetSetFlag(EFlag inFlag, bool inState, float inDuration = 0.0f)
    {
        _flags[inFlag] = inState;

        // Start a duration coroutine, and overwrite any earlier duration coroutine of the same type
        if (inDuration > 0)
            _flagHandles[inFlag] = Timing.RunCoroutineSingleton(HandleDuration(inFlag, inDuration, inState), _flagHandles[inFlag], SingletonBehavior.Overwrite);

        // If the flag is set without a duration and a duration coroutine is going, kill the coroutine
        else
            Timing.KillCoroutines(_flagHandles[inFlag]);
    }

    IEnumerator<float> HandleDuration(EFlag inFlag, float inDuration, bool inState)
    {
        yield return Timing.WaitForSeconds(inDuration);
        SetFlag(inFlag, !inState);
    }
}


public enum EFlag
{
    Reloading
}