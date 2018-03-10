using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public void Initialize(Player inPlayer)
    {
        GetComponent<FollowCamera>().SetTargetPlayer(inPlayer);
        GetComponent<CameraShaker>().Initialize(inPlayer.healthComponent);
        GetComponent<CameraPuncher>().Initialize(inPlayer.healthComponent);
    }
}