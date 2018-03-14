using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerRespawnComponent : MonoBehaviour
{
    [SerializeField] float _respawnTime = 3.0f;

    Player            _player;
    SpawnPointManager _spawnPointManager;
    GameObject        _graphicsGO;

    public event Action OnSpawn;
    public event Action OnDespawn;


    public void ManualAwake()
    {
        _player            = GetComponent<Player>();
        _graphicsGO        = _player.graphicsGO;
        _spawnPointManager = FindObjectOfType<SpawnPointManager>();


        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        _player.OnPlayerCreated         += () => Timing.RunCoroutine(_HandleRespawn());
        _player.healthComponent.OnDeath += () => Timing.RunCoroutine(_HandleRespawn());

        _player.healthComponent.OnDeath += Despawn;
    }


    IEnumerator<float> _HandleRespawn()
    {
        yield return Timing.WaitForSeconds(_respawnTime);
        Spawn();
    }

    void Spawn()
    {
        transform.position = _spawnPointManager.GetRandomSpawnPoint(_player.teamComponent.team);
        _player.healthComponent.enabled = true;
        _graphicsGO.SetActive(true);

        OnSpawn?.Invoke();
    }

    void Despawn()
    {
        _player.healthComponent.enabled = false;
        _graphicsGO.SetActive(false);

        OnDespawn?.Invoke();
    }
}
