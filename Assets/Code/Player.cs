using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class Player : Photon.MonoBehaviour
{
    public PlayerMovementComponent  _movementComponent  { get; private set; }
    public PlayerCollisionComponent _collisionComponent { get; private set; }
    public PlayerHealthComponent    _healthComponent    { get; private set; }
    public PlayerInputComponent     _inputComponent     { get; private set; }
    public PlayerTeamComponent      _teamComponent      { get; private set; }
    public PlayerWeaponComponent    _weaponComponent    { get; private set; }
    

    void Awake()
    {
        if (!photonView.isMine)
            return;

        _movementComponent  = GetComponent<PlayerMovementComponent>();
        _collisionComponent = GetComponent<PlayerCollisionComponent>();
        _healthComponent    = GetComponent<PlayerHealthComponent>();
        _inputComponent     = GetComponent<PlayerInputComponent>();
        _teamComponent      = GetComponent<PlayerTeamComponent>();
        _weaponComponent    = GetComponent<PlayerWeaponComponent>();

        FollowCamera.SetTarget(transform);
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        _movementComponent.ManualUpdate();
        _collisionComponent.ManualUpdate();
        _weaponComponent.ManualUpdate();
    }
}