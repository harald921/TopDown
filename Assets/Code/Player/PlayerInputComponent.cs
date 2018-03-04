using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputComponent : MonoBehaviour
{
	public struct SInput
    {
        public Vector3 movementDirection;
        public Vector3 aimTarget;
        public bool pullWeaponTrigger;
        public bool releaseWeaponTrigger;
        public bool pickUpWeapon;
        public bool dropWeapon;
    }

    SInput _input;
    public SInput input => _input;


    public void ManualUpdate()
    {
        _input = new SInput()
        {
            movementDirection    = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")),
            aimTarget            = GetAimTarget(),
            pullWeaponTrigger    = Input.GetMouseButton(0),
            releaseWeaponTrigger = Input.GetMouseButtonUp(0),
            pickUpWeapon         = Input.GetKeyDown(KeyCode.E),
            dropWeapon           = Input.GetKeyDown(KeyCode.G)
        };
    }

    Vector3 GetAimTarget()
    {
        Vector3 aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.localScale.y, aimTarget.z);

        return aimTarget;
    }
}