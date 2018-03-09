using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Photon.MonoBehaviour
{
    [SerializeField] protected Type _type;
    protected PlayerInputComponent _inputComponent;

    // External
    public void PickUp(Player inPlayer)
    {
        _inputComponent = inPlayer.inputComponent;
    }

    public void Drop()
    {
        _inputComponent = null;
    }


    public enum Type
    {
        None,

        Ballistic,
        Plasma
    }
}
