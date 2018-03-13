using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    Dictionary<int, List<SpawnPoint>> _spawnPoints = new Dictionary<int, List<SpawnPoint>>();


    void Awake()
    {
        GetSpawnPointsAndSetDictionary();
    }


    void GetSpawnPointsAndSetDictionary()
    {
        SpawnPoint[] spawnPoints = GetComponentsInChildren<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            // If spawnpoints doesn't contain the team id, create a new list to hold points of that ID
            if (!_spawnPoints.ContainsKey(spawnPoint.team))
                _spawnPoints.Add(spawnPoint.team, new List<SpawnPoint>());
                
            _spawnPoints[spawnPoint.team].Add(spawnPoint);
        }
    }


    public Vector3 GetRandomSpawnPoint(int inTeam)
    {
        if (!_spawnPoints.ContainsKey(inTeam))
            throw new System.ArgumentException("Something called SpawnPointManager.GetRandomSpawnPoint(), with the parameter " + inTeam + ". This team doesn't exist.");

        List<SpawnPoint> spawnPoints = _spawnPoints[inTeam];
        int randomIndex = Random.Range(0, _spawnPoints.Count);

        Vector3 spawnPoint = spawnPoints[randomIndex].transform.position;

        return new Vector3()
        {
            x = spawnPoint.x,
            y = 1,
            z = spawnPoint.z
        };
    }
}
