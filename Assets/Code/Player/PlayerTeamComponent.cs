using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeamComponent : MonoBehaviour
{
    [SerializeField] int _team = 0;
    public int team => _team;
}