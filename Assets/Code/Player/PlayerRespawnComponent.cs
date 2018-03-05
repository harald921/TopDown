using System;
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

    PlayerHealthComponent _healthComponent;
    PlayerFlagComponent _flagComponent;

    public event Action OnRespawn;
    

    void Awake()
    {
        _healthComponent = GetComponent<PlayerHealthComponent>();
        _flagComponent = GetComponent<PlayerFlagComponent>();

        _graphicsGO = GetComponent<Player>().graphicsGO;

        _spawnLocations = GetAllSpawnLocations();

        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        _healthComponent.OnDeath += () => Timing.RunCoroutine(_HandleRespawn());

        // Despawning and Spawning
        _healthComponent.OnDeath += Despawn;
        OnRespawn += Spawn;

        // Death state
        _healthComponent.OnDeath += () => _flagComponent.SetFlag(EFlag.Dead, true);
        OnRespawn += () => _flagComponent.SetFlag(EFlag.Dead, false);
    }


    SpawnLocation[] GetAllSpawnLocations()
    {
        return _spawnLocationsParent.GetComponentsInChildren<SpawnLocation>();
    }

    IEnumerator<float> _HandleRespawn()
    {
        yield return Timing.WaitForSeconds(_respawnTime);
        OnRespawn?.Invoke();
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
    }

    Vector3 GetRandomSpawnPosition()
    {
        SpawnLocation spawnLocation = _spawnLocations[UnityEngine.Random.Range(0, _spawnLocations.Length)];

        return new Vector3(spawnLocation.transform.position.x, 1, spawnLocation.transform.position.z);
    }
}
