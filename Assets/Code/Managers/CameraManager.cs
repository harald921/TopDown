using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static CameraManager _instance;
    public static CameraManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<CameraManager>();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    [SerializeField] FollowCamera _followCamera;
    public FollowCamera followCamera => _followCamera;

    [SerializeField] CameraShaker  _cameraShaker;
    public CameraShaker cameraShaker => _cameraShaker;

    [SerializeField] CameraPuncher _cameraPuncher;
    public CameraPuncher cameraPuncher => _cameraPuncher;

    Camera _mainCamera;
    public Camera mainCamera => _mainCamera;


    void Awake()
    {
        _instance = this;
        Initialize();
    }

    void Initialize()
    {
        _mainCamera = Camera.main;
    }
}