using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.MonoBehaviour
{
    void Awake()
    {
        ConnectToServer();

    }

    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings("0.01");
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
        PhotonNetwork.Instantiate("Networking/Player", Vector3.up, Quaternion.identity, 0);
    }

    void OnPhotonPlayerConnected()
    {
        PhotonNetwork.Instantiate("Networking/Player", Vector3.up, Quaternion.identity, 0);
    }
}
