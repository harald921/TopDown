using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class PlayerFlagComponent : Photon.MonoBehaviour
{
    // Private
    Dictionary<EFlag, bool> _flags = new Dictionary<EFlag, bool>();
    Dictionary<EFlag, CoroutineHandle> _flagHandles = new Dictionary<EFlag, CoroutineHandle>();

    List<EFlag> _runningFlagDurations = new List<EFlag>();


    void Awake()
    {
        foreach (EFlag flag in Enum.GetValues(typeof(EFlag)))
        {
            _flags.Add(flag, false);
            _flagHandles.Add(flag, new CoroutineHandle());
        }
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
        {
            _runningFlagDurations.Add(inFlag);
            Timing.RunCoroutineSingleton(HandleDuration(inFlag, inDuration, inState), _flagHandles[inFlag], SingletonBehavior.Overwrite);
        }

        // If the flag is set without a duration and a duration coroutine is going, kill the coroutine
        else if (_runningFlagDurations.Contains(inFlag))
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
    Dead,
}