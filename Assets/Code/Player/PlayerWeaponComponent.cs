using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponComponent : Photon.MonoBehaviour
{
    [SerializeField] Transform _handTransform;
    [SerializeField] float _weaponPickupRadius = 3.0f;

    Weapon _heldWeapon;
    public Weapon heldWeapon => _heldWeapon;

    Player                   _player;
    PlayerCollisionComponent _collisionComponent;
    PlayerInputComponent     _inputComponent;

    public event Action OnWeaponFire;


    public void ManualAwake()
    {
        _player             = GetComponent<Player>();
        _collisionComponent = _player.collisionComponent;
        _inputComponent     = _player.inputComponent;

        if (!photonView.isMine)
            return;

        _player.healthComponent.OnDeath += () => { if (_heldWeapon) photonView.RPC("DropWeapon", PhotonTargets.All); };
    }

    public void ManualUpdate()
    {
        HandleWeaponPickup();
    }


    void HandleWeaponPickup()
    {
        if (_inputComponent.input.pickUpWeapon)
        {
            Weapon nearbyWeapon = GetClosestWeapon();
            if (nearbyWeapon)
            {
                if (_heldWeapon)
                    photonView.RPC("DropWeapon", PhotonTargets.All);

                photonView.RPC("PickupWeapon", PhotonTargets.All, nearbyWeapon.photonView.viewID);
            }
        }

        if (_inputComponent.input.dropWeapon)
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
        _heldWeapon.PickUp(_inputComponent);
        _heldWeapon.OnFire += OnWeaponFire;
    }

    [PunRPC]
    void DropWeapon()
    {
        Rigidbody weaponRigidbody = _heldWeapon.GetComponent<Rigidbody>();
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.AddTorque(_heldWeapon.transform.forward * 4.0f);

        _heldWeapon.OnFire -= OnWeaponFire;
        _heldWeapon.Drop();
        _heldWeapon.transform.SetParent(null);
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
        Collider[] hitColliders = _collisionComponent.CheckCollision(_weaponPickupRadius);

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
}