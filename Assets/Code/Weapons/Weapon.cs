using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Photon.MonoBehaviour
{
    [SerializeField] Stats _stats;
    public Stats stats => _stats;
     
    [SerializeField] protected Transform _muzzleTransform;
    public Transform muzzleTransform => _muzzleTransform;

    public WeaponAmmoComponent ammoComponent { get; private set; }
    public WeaponFireComponent fireComponent { get; private set; }
    public WeaponFlagComponent flagComponent { get; private set; }

    PlayerInputComponent _inputComponent;
    public PlayerInputComponent inputComponent => _inputComponent;

    public event Action OnDropped;
    public event Action OnPickedUp;


    void Awake()
    {
        ammoComponent = GetComponent<WeaponAmmoComponent>();
        fireComponent = GetComponent<WeaponFireComponent>();
        flagComponent = GetComponent<WeaponFlagComponent>();

        InvokeManualAwakes();
    }

    void Update()
    {
        InvokeManualUpdates();
    }

    void InvokeManualAwakes()
    {
        ammoComponent.ManualAwake();
        fireComponent.ManualAwake();
        flagComponent.ManualAwake();
    }

    void InvokeManualUpdates()
    {
        fireComponent.ManualUpdate();
    }

    public void PickUp(PlayerInputComponent inInputComponent)
    {
        _inputComponent = inInputComponent;
        OnPickedUp?.Invoke();
    }

    public void Drop()
    {
        _inputComponent = null;
        OnDropped?.Invoke();
    }

    [Serializable]
    public struct Stats
    {
        public string name;
    }

    public enum Type
    {
        None,

        Ballistic,
        Plasma
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
 *  - class UIComponent
 */
