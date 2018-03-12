﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerRespawnComponent : MonoBehaviour
{
    [SerializeField] Transform _spawnLocationsParent;
    [SerializeField] float _respawnTime = 3.0f;

    GameObject _graphicsGO;
    SpawnLocation[] _spawnLocations;

    Player _player;
    PlayerHealthComponent _healthComponent;

    public event Action OnSpawn;


    void Awake()
    {
        _player = GetComponent<Player>();
        _healthComponent = GetComponent<PlayerHealthComponent>();

        _graphicsGO = GetComponent<Player>().graphicsGO;

        _spawnLocations = GetAllSpawnLocations();

        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        _player.OnPlayerCreated  += () => Timing.RunCoroutine(_HandleRespawn());
        _healthComponent.OnDeath += () => Timing.RunCoroutine(_HandleRespawn());

        _healthComponent.OnDeath += Despawn;
    }


    SpawnLocation[] GetAllSpawnLocations()
    {
        return GameObject.Find("SpawnLocations").GetComponentsInChildren<SpawnLocation>();
    }

    IEnumerator<float> _HandleRespawn()
    {
        yield return Timing.WaitForSeconds(_respawnTime);
        Spawn();
    }

    void Despawn()
    {
        _healthComponent.enabled = false;
        _graphicsGO.SetActive(false);
    }

    void Spawn()
    {
        transform.position = GetRandomSpawnPosition();
        _healthComponent.enabled = true;
        _graphicsGO.SetActive(true);

        OnSpawn?.Invoke();
    }

    Vector3 GetRandomSpawnPosition()
    {
        SpawnLocation spawnLocation = _spawnLocations[UnityEngine.Random.Range(0, _spawnLocations.Length)];

        return new Vector3(spawnLocation.transform.position.x, 1, spawnLocation.transform.position.z);
    }
}
