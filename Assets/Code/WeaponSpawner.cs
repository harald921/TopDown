using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : Photon.MonoBehaviour
{
    [SerializeField] GameObject _weaponPrefab;

    [SerializeField] float _spawnHeight    = 1.0f;
    [SerializeField] float _spawnInterval  = 60.0f; // Respawn interval in seconds
    [SerializeField] float _rotationOffset = 90.0f; // Rotation around forward in degrees

    float _timer = 0.0f;


    void Awake()
    {
        _timer = _spawnInterval;    
    }

    void Update()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (transform.childCount > 0)
            return;

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            SpawnWeapon();
            _timer = _spawnInterval;
        }
    }

    void SpawnWeapon()
    {
        GameObject spawnedWeapon = PhotonNetwork.Instantiate(_weaponPrefab.name, transform.position + (Vector3.up * _spawnHeight), Quaternion.identity, 0);
        photonView.RPC("NetSetParent", PhotonTargets.AllBuffered, spawnedWeapon.GetPhotonView().viewID, photonView.viewID);
    }

    [PunRPC]
    void NetSetParent(int inChildViewID, int inParentViewID)
    {
        Transform weaponTransform  = PhotonView.Find(inChildViewID).transform;
        Transform spawnerTransform = PhotonView.Find(inParentViewID).transform;

        weaponTransform.transform.Rotate(weaponTransform.forward, 90);
        weaponTransform.SetParent(spawnerTransform);
    }
}
