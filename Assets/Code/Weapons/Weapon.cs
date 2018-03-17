using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Photon.MonoBehaviour
{
    [SerializeField] protected Transform _muzzleTransform;
    [SerializeField] Stats _stats;
    public Stats stats => _stats;

    protected int _currentAmmo;
    public int currentAmmo => _currentAmmo;

    protected PlayerInputComponent _inputComponent;

    public event Action OnFire;
    public event Action OnReloadStart;
    public event Action OnReloadFinish;


    public virtual void PickUp(PlayerInputComponent inInputComponent)
    {
        _inputComponent = inInputComponent;
    }

    public virtual void Drop()
    {
        _inputComponent = null;

        OnFire          = null;
        OnReloadStart   = null;
        OnReloadFinish  = null;
    }

    protected void TryInvokeOnFire()
    {
        OnFire?.Invoke();
    }
    protected void TryInvokeReloadStart()
    {
        OnReloadStart?.Invoke();
    }
    protected void TryInvokeReloadFinish()
    {
        OnReloadFinish?.Invoke();
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
        public string name;

        [Space(5)]
        public float fireTime;
        public float spread;

        [Space(5)]
        public float recoilMoveSlow;

        [Space(5)]
        public int   maxAmmo;
        public float reloadTime;
    }
}


/* Component Based Weapon
 * 
 *  - class Weapon
 *      * _muzzleTransform
 *      * PullTrigger()
 *      
 *  - class AmmoComponent
 *      - class ExternalMagazineAmmoComponent
 *      - class InternalMagazineAmmoComponent
 *      - class BatteryAmmoComponent       
 * 
 *  - class FireComponent
 *      - class HitscanFireComponent
 *      - class ProjectileFireComponent
 *  
 *  - class FlagComponent
 *      * Allows the other components to add flags that components can check for
 *  
 *  - class OverHeatComponent
 *      * Makes the weapon overheat if fired too often. 
 *      
 *  - class ChargeUpComponent
 *      * Makes the weapon have to charge up before firing
 *  
 */
