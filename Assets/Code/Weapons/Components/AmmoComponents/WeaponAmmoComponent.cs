﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WeaponAmmoComponent : MonoBehaviour
{
    public event Action OnReloadStart;
    public event Action OnReloadStop;

    public virtual void ManualAwake() { }
    
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
