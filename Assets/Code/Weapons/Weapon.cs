using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Photon.MonoBehaviour
{
    [SerializeField] protected Type _type;
    [SerializeField] protected Transform _muzzleTransform;

    protected PlayerInputComponent _inputComponent;

    public event Action OnFire;

    public abstract void PickUp(PlayerInputComponent inInputComponent);
    public abstract void Drop();

    protected void TryInvokeOnFire()
    {
        OnFire?.Invoke();
    }


    public enum Type
    {
        None,

        Ballistic,
        Plasma
    }
}
