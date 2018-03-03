using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.MonoBehaviour
{
    const string _gameVersion = "0.01";

    void Awake()
    {
        PhotonNetwork.sendRate = 64;
        PhotonNetwork.sendRateOnSerialize = 64;

        ConnectToServer();
    }


    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }

    void OnConnectedToMaster()
    {
        JoinRoom();
        Debug.Log("Connected to Photon");
    }

    void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom("debugRoom", new RoomOptions(), TypedLobby.Default);
    }

    void OnCreatedRoom()
    {
        Debug.Log("No room exists, creating new");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        PhotonNetwork.Instantiate("Player", Vector3.up, Quaternion.identity, 0);
    }

    public static float CalculateNetDelta(double inTimestamp)
    {
        return (float)(PhotonNetwork.time - inTimestamp);
    }
}
