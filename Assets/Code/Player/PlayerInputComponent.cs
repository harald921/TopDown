using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputComponent : Photon.MonoBehaviour
{
    [SerializeField] LayerMask _layerMask;

    SInput _input;
    public SInput input => _input;

    Camera _mainCamera;
    

    void Awake()
    {
        if (!photonView.isMine)
            return;

        _mainCamera = Camera.main;
    }

    public void ManualUpdate()
    {
        if (!photonView.isMine)
            return;

        _input = new SInput()
        {
            movementDirection    = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")),
            aimTarget            = GetAimTarget(),
            mousePosition        = Input.mousePosition,
            pullWeaponTrigger    = Input.GetMouseButton(0),
            releaseWeaponTrigger = Input.GetMouseButtonUp(0),
            pickUpWeapon         = Input.GetKeyDown(KeyCode.E),
            dropWeapon           = Input.GetKeyDown(KeyCode.G)
        };
    }

    Vector3 GetAimTarget()
    {
        Plane aimTargetPlane = new Plane(Vector3.up, Vector3.up); // Second parameter should be the height of the players weapon
        Ray mouseThroughCameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        float distanceToHit;
        aimTargetPlane.Raycast(mouseThroughCameraRay, out distanceToHit);

        Vector3 aimTarget = mouseThroughCameraRay.GetPoint(distanceToHit);

        return aimTarget;
    }

    public struct SInput
    {
        public Vector3 movementDirection;
        public Vector3 aimTarget;
        public Vector3 mousePosition;
        public bool pullWeaponTrigger;
        public bool releaseWeaponTrigger;
        public bool pickUpWeapon;
        public bool dropWeapon;
    }
}