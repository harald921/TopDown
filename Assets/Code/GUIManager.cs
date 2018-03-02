using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    static GUIManager _instance;
    public static GUIManager instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType<GUIManager>();

            return _instance;
        }
    }

    [SerializeField] ShieldBar _shieldBar;
    public ShieldBar shieldBar { get { return _shieldBar; } }

    void Awake()
    {
        _instance = this;
    }
}
