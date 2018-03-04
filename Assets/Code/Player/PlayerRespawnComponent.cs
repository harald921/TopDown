using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnComponent : MonoBehaviour
{
    [SerializeField] Transform _spawnLocationsParent;
    [SerializeField] float _respawnTime = 3.0f;

    PlayerHealthComponent _playerHealthComponent;
    MeshRenderer _meshRenderer;
    SpawnLocation[] _spawnLocations;


    void Awake()
    {
        _playerHealthComponent = GetComponent<PlayerHealthComponent>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _spawnLocations = GetAllSpawnLocations();

        _playerHealthComponent.OnDeath += () => StartCoroutine(HandleRespawn());
    }


    SpawnLocation[] GetAllSpawnLocations()
    {
        return _spawnLocationsParent.GetComponentsInChildren<SpawnLocation>();
    }

    IEnumerator HandleRespawn()
    {
        DespawnPlayer();
        yield return new WaitForSeconds(_respawnTime);
        RespawnPlayer();
    }

    void DespawnPlayer()
    {
        _playerHealthComponent.enabled = false;
        _meshRenderer.enabled = false;
    }

    void RespawnPlayer()
    {
        transform.position = GetRandomSpawnPosition();
        _playerHealthComponent.enabled = true;
        _meshRenderer.enabled = true;
    }


    Vector3 GetRandomSpawnPosition()
    {
        SpawnLocation spawnLocation = _spawnLocations[Random.Range(0, _spawnLocations.Length)];

        return new Vector3(spawnLocation.transform.position.x, 1, spawnLocation.transform.position.z);
    }
}
