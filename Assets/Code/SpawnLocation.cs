using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] int _team = 0;
    public int team { get { return _team; } }
}
