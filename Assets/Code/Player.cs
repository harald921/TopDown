using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Photon.MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;
    [SerializeField] Transform _handTransform;

    [Header("Health")]
    [SerializeField] int _maxHealth = 100;

    [Header("Movement")]
    [SerializeField] float _moveSpeed = 2.5f;

    [Header("Rotation")]
    [SerializeField] float _rotationSpeed = 10.0f;

    [Header("Collision")]
    [SerializeField] LayerMask _collidesWith;
    [SerializeField] float _environmentCollisionRange = 1.0f;
    [SerializeField] float _weaponPickupRadius = 3.0f;

    [Header("Team")]
    [SerializeField] int _team = 0;

    int _currentHealth;

    Rigidbody _rigidBody;
    Camera _mainCamera;
    Collider _collider;
    Weapon _heldWeapon;

    public event Action<int> OnHurt;
    public event Action<int> OnHealed;
    public event Action OnDeath;

    void Awake()
    {
        _currentHealth = _maxHealth;

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

        if (Input.GetKeyDown(KeyCode.E))
            ModifyHealth(-10);

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
                    photonView.RPC("DropWeapon", PhotonTargets.All);

                photonView.RPC("PickupWeapon", PhotonTargets.All, nearbyWeapon.photonView.viewID);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            if (_heldWeapon)
                photonView.RPC("DropWeapon", PhotonTargets.All);
    }

    [PunRPC]
    void PickupWeapon(int inViewID)
    {
        Weapon weaponToPickup = PhotonView.Find(inViewID).GetComponent<Weapon>();
        weaponToPickup.transform.SetParent(_handTransform);
        weaponToPickup.transform.forward = _handTransform.forward;
        weaponToPickup.transform.localPosition = Vector3.zero;

        weaponToPickup.GetComponent<Rigidbody>().isKinematic = true;

        _heldWeapon = weaponToPickup;
    }

    [PunRPC]
    void DropWeapon()
    {
        Rigidbody weaponRigidbody = _heldWeapon.GetComponent<Rigidbody>();
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.AddTorque(_heldWeapon.transform.forward * 4.0f);

        _heldWeapon.transform.SetParent(null);
        _heldWeapon.ReleaseTrigger();
        _heldWeapon = null;
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
            {
                if (!hitWeapon.transform.parent)
                    nearbyWeapons.Add(hitWeapon);

                else if (hitWeapon.transform.parent.GetComponent<WeaponSpawner>())
                    nearbyWeapons.Add(hitWeapon);
            }
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
        return Quaternion.LookRotation(GetAimTarget() - transform.position);
    }

    Vector3 GetAimTarget()
    {
        Vector3 aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.localScale.y, aimTarget.z);

        return aimTarget;
    }

    void HandleMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveSpeed * Time.deltaTime;
        transform.position += movement;

        ResolveCollision(CheckCollision(_environmentCollisionRange));
    }

    Collider[] CheckCollision(float inRange)
    {
        return Physics.OverlapSphere(transform.position - Vector3.up, inRange);
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

    [PunRPC]
    void ModifyHealth(int inChange)
    {
        _currentHealth += inChange;

        if (inChange < 0)
        {
            if (OnHurt != null)
                OnHurt(inChange);
        }

        else
        {
            if (OnHealed != null)
                OnHealed(inChange);
        }

        if (_currentHealth <= 0)
            if (OnDeath != null)
                OnDeath();
    }
}