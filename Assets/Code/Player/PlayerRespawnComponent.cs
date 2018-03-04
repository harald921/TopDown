using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerRespawnComponent : MonoBehaviour
{
    [SerializeField] Transform _spawnLocationsParent;
    [SerializeField] float _respawnTime = 3.0f;

    MeshRenderer _meshRenderer;
    SpawnLocation[] _spawnLocations;

    PlayerHealthComponent _healthComponent;

    public event Action OnRespawn;
    

    void Awake()
    {
        _healthComponent = GetComponent<PlayerHealthComponent>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _spawnLocations = GetAllSpawnLocations();

        _healthComponent.OnDeath += () => Timing.RunCoroutine(_HandleRespawn());
    }


    SpawnLocation[] GetAllSpawnLocations()
    {
        return _spawnLocationsParent.GetComponentsInChildren<SpawnLocation>();
    }

    IEnumerator<float> _HandleRespawn()
    {
        Despawn();
        yield return Timing.WaitForSeconds(_respawnTime);
        Spawn();
    }

    void Despawn()
    {
        _healthComponent.enabled = false;
        _meshRenderer.enabled = false;
    }

    void Spawn()
    {
        transform.position = GetRandomSpawnPosition();
        _healthComponent.enabled = true;
        _meshRenderer.enabled = true;

        OnRespawn?.Invoke();
    }

    Vector3 GetRandomSpawnPosition()
    {
        SpawnLocation spawnLocation = _spawnLocations[UnityEngine.Random.Range(0, _spawnLocations.Length)];

        return new Vector3(spawnLocation.transform.position.x, 1, spawnLocation.transform.position.z);
    }
}
