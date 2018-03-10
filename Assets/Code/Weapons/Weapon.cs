using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Photon.MonoBehaviour
{
    [SerializeField] protected Type _type;
    [SerializeField] protected Transform _muzzleTransform;
    [SerializeField] Stats _stats;
    public Stats stats => _stats;

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

    [Serializable]
    public struct Stats
    {
        [Space(5)]
        public int damage;
        public float fireTime;
        public float spread;

        [Space(5)]
        public float recoilMoveSlow;

        [Space(5)]
        public int maxAmmo;
        public float reloadTime;
    }
}
