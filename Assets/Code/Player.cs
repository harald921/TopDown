using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class Player : Photon.MonoBehaviour
{
    [SerializeField] GameObject _graphicsGO;
    public GameObject graphicsGO => _graphicsGO;

    public PlayerMovementComponent  movementComponent  { get; private set; }
    public PlayerCollisionComponent collisionComponent { get; private set; }
    public PlayerHealthComponent    healthComponent    { get; private set; }
    public PlayerInputComponent     inputComponent     { get; private set; }
    public PlayerTeamComponent      teamComponent      { get; private set; }
    public PlayerWeaponComponent    weaponComponent    { get; private set; }
    public PlayerFlagComponent      flagComponent      { get; private set; }
    public PlayerRespawnComponent   respawnComponent   { get; private set; }

    public event Action OnPlayerCreated;


    void Awake()
    {
        SetPlayerComponentFields();

        InvokeManualAwakes();

        if (!photonView.isMine)
            return;

        FindObjectOfType<CameraManager>().Initialize(this);
    }

    void Start()
    {
        OnPlayerCreated?.Invoke();
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        InvokeManualUpdates();
    }

    
    void SetPlayerComponentFields()
    {
        movementComponent  = GetComponent<PlayerMovementComponent>();
        collisionComponent = GetComponent<PlayerCollisionComponent>();
        healthComponent    = GetComponent<PlayerHealthComponent>();
        inputComponent     = GetComponent<PlayerInputComponent>();
        teamComponent      = GetComponent<PlayerTeamComponent>();
        weaponComponent    = GetComponent<PlayerWeaponComponent>();
        flagComponent      = GetComponent<PlayerFlagComponent>();
        respawnComponent   = GetComponent<PlayerRespawnComponent>();
    }

    void InvokeManualAwakes()
    {
        movementComponent.ManualAwake();
        weaponComponent.ManualAwake();
        flagComponent.ManualAwake();
        respawnComponent.ManualAwake();
    }

    void InvokeManualUpdates()
    {
        inputComponent.ManualUpdate();
        movementComponent.ManualUpdate();
        collisionComponent.ManualUpdate();
        weaponComponent.ManualUpdate();
    }
}