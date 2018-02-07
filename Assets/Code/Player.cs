using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;
    [SerializeField] Transform _handTransform;

    [Header("Movement")]
    [SerializeField] float _moveSpeed = 2.5f;

    [Header("Rotation")]
    [SerializeField] float _rotationSpeed = 10.0f;

    [Header("Collision")]
    [SerializeField] LayerMask _collidesWith;
    [SerializeField] float _environmentCollisionRange = 1.0f;
    [SerializeField] float _weaponPickupRadius = 3.0f;

    Rigidbody _rigidBody;
    Camera _mainCamera;
    Collider _collider;

    Weapon _heldWeapon;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();    
        _mainCamera = Camera.main;

        if (!photonView.isMine)
            return; 

        FollowCamera.SetTarget(transform);
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        HandleMovement();
        HandleRotation();

        HandleShooting();
        HandleWeaponPickup();
    }

    void HandleWeaponPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Weapon nearbyWeapon = GetClosestWeapon();
            if (nearbyWeapon)
            {
                if (_heldWeapon)
                    ThrowWeapon(200.0f);

                PickupWeapon(nearbyWeapon);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            if (_heldWeapon)
                ThrowWeapon(600.0f);
    }

    void PickupWeapon(Weapon inWeaponToPickup)
    {
        inWeaponToPickup.transform.SetParent(_handTransform);
        inWeaponToPickup.transform.forward = _handTransform.forward;
        inWeaponToPickup.transform.localPosition = Vector3.zero;

        inWeaponToPickup.GetComponent<Rigidbody>().isKinematic = true;

        _heldWeapon = inWeaponToPickup;
    }

    Weapon DropWeapon()
    {
        Weapon weaponToDrop = _heldWeapon;

        _heldWeapon.GetComponent<Rigidbody>().isKinematic = false;
        _heldWeapon.transform.SetParent(null);
        _heldWeapon.ReleaseTrigger();
        _heldWeapon = null;

        return weaponToDrop;
    }

    void ThrowWeapon(float inStrength)
    {
        Weapon droppedWeapon = DropWeapon();

        droppedWeapon.GetComponent<Rigidbody>().AddForce((CalculateTargetRotation() * Vector3.forward) * inStrength);
    }

    Weapon GetClosestWeapon()
    {
        Weapon closestWeapon = null;
        float distToClosestWeapon = Mathf.Infinity;
        foreach (Weapon nearbyWeapon in GetNearbyWeapons())
        {
            float distToNearbyWeapon = Vector3.Distance(transform.position, nearbyWeapon.transform.position);
            if (distToNearbyWeapon > distToClosestWeapon)
                continue;

            closestWeapon = nearbyWeapon;
            distToClosestWeapon = distToNearbyWeapon;
        }

        return closestWeapon;
    }

    List<Weapon> GetNearbyWeapons()
    {
        Collider[] hitColliders = CheckCollision(_weaponPickupRadius);

        List<Weapon> nearbyWeapons = new List<Weapon>();
        foreach (Collider hitCollider in hitColliders)
        {
            Weapon hitWeapon = hitCollider.GetComponent<Weapon>();
            if (hitWeapon)
                if (!hitWeapon.transform.parent)
                    nearbyWeapons.Add(hitWeapon);
        }

        return nearbyWeapons;
    }


    void HandleShooting()
    {
        if (!_heldWeapon)
            return;

        if (Input.GetMouseButton(0))
            _heldWeapon.PullTrigger();

        if (Input.GetMouseButtonUp(0))
            _heldWeapon.ReleaseTrigger();
    }


    void HandleRotation()
    {
        _rotationTransform.rotation = Quaternion.RotateTowards(_rotationTransform.rotation, CalculateTargetRotation(), _rotationSpeed * Time.deltaTime);
    }


    Quaternion CalculateTargetRotation()
    {
        Vector3 aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.localScale.y, aimTarget.z);

        return Quaternion.LookRotation(aimTarget - transform.position);
    }


    void HandleMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveSpeed * Time.deltaTime;
        transform.position += movement;


        ResolveCollision(CheckCollision(_environmentCollisionRange));
    }

    Collider[] CheckCollision(float inRange)
    {
        return Physics.OverlapSphere(transform.position - Vector3.up * 1, inRange);
    }

    void ResolveCollision(Collider[] hitColliders)
    {
        foreach (Collider hitCollider in hitColliders)
        {
            if (!_collidesWith.Contains(hitCollider.gameObject.layer))
                continue;

            Vector3 otherPos = hitCollider.transform.position;
            Quaternion otherRot = hitCollider.transform.rotation;

            Vector3 correctionDir;
            float correctionDist;

            if (Physics.ComputePenetration(_collider, transform.position, transform.rotation, hitCollider, hitCollider.transform.position, hitCollider.transform.rotation, out correctionDir, out correctionDist))
                transform.position += correctionDir * correctionDist;
        }
    }
}
