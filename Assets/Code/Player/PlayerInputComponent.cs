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
            weaponTriggerPulled  = Input.GetMouseButton(0),
            reloadWeapon         = Input.GetKeyDown(KeyCode.R),
            pickUpWeapon         = Input.GetKeyDown(KeyCode.E),
            dropWeapon           = Input.GetKeyDown(KeyCode.G)
        };
    }

    Vector3 GetAimTarget()
    {
        // Create a plane at the heigh of the player's eyes
        Plane aimTargetPlane = new Plane(Vector3.up, Vector3.up);

        // Create a ray going through the camera at the mouseposition, and get the distance at which the ray contacts the plane
        Ray mouseThroughCameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        float distanceToHit;
        aimTargetPlane.Raycast(mouseThroughCameraRay, out distanceToHit);

        // The point that is the distance along the ray is the aim target
        return mouseThroughCameraRay.GetPoint(distanceToHit);
    }

    public struct SInput
    {
        public Vector3 movementDirection;
        public Vector3 aimTarget;
        public Vector3 mousePosition;
        public bool weaponTriggerPulled;
        public bool pickUpWeapon;
        public bool reloadWeapon;
        public bool dropWeapon;
    }
}