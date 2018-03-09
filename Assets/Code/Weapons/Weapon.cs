using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Photon.MonoBehaviour
{
    [SerializeField] protected Type _type;
    [SerializeField] protected Transform _muzzleTransform;

    protected PlayerInputComponent _inputComponent;

    public abstract void PickUp(PlayerInputComponent inInputComponent);
    public abstract void Drop();


    public enum Type
    {
        None,

        Ballistic,
        Plasma
    }

    public class Input
    {
        public bool triggerPulled;
        public bool reload;
    }
}
