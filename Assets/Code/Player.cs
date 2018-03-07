using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class Player : Photon.MonoBehaviour
{
    [SerializeField] GameObject _graphicsGO;
    public GameObject graphicsGO => _graphicsGO;

    public PlayerMovementComponent  movementComponent   { get; private set; }
    public PlayerCollisionComponent collisionComponent  { get; private set; }
    public PlayerHealthComponent    healthComponent     { get; private set; }
    public PlayerInputComponent     inputComponent      { get; private set; }
    public PlayerTeamComponent      teamComponent       { get; private set; }
    public PlayerWeaponComponent    weaponComponent     { get; private set; }
    public PlayerFlagComponent      flagComponent       { get; private set; }


    void Awake()
    {
        if (!photonView.isMine)
            return;

        movementComponent  = GetComponent<PlayerMovementComponent>();
        collisionComponent = GetComponent<PlayerCollisionComponent>();
        healthComponent    = GetComponent<PlayerHealthComponent>();
        inputComponent     = GetComponent<PlayerInputComponent>();
        teamComponent      = GetComponent<PlayerTeamComponent>();
        weaponComponent    = GetComponent<PlayerWeaponComponent>();
        flagComponent      = GetComponent<PlayerFlagComponent>();

        movementComponent.ManualAwake();
        weaponComponent.ManualAwake();

        FindObjectOfType<FollowCamera>().SetTargetPlayer(this);
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        inputComponent.ManualUpdate();
        movementComponent.ManualUpdate();
        collisionComponent.ManualUpdate();
        weaponComponent.ManualUpdate();
    }
}