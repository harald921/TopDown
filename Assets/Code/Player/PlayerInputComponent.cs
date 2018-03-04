using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputComponent : MonoBehaviour
{
    [SerializeField] LayerMask _layerMask; 

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
        Vector3 aimTarget = Vector3.zero;

        RaycastHit[] hits = Physics.RaycastAll(CameraManager.instance.mainCamera.ScreenPointToRay(Input.mousePosition));

        foreach (RaycastHit hit in hits)
            if (_layerMask.Contains(hit.collider.gameObject.layer))
            {
                aimTarget = hit.point;
                break;
            }

        aimTarget = new Vector3(aimTarget.x, 1, aimTarget.z);

        return aimTarget;
    }
}