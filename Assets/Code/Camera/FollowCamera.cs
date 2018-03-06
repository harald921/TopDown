using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Vector3 _cameraOffset = new Vector3(0.0f, 15.0f, -5.0f);
    [SerializeField] float   _smoothTime   = 0.1f;

    Vector3 _velocity;
    Vector3 _targetPosition => _targetPlayer.transform.position + _cameraOffset;

    Camera _camera;
    Player _targetPlayer;
    PlayerInputComponent _inputComponent;


    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!_targetPlayer)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _smoothTime);
    }


    public void SetTargetPlayer(Player inPlayer)
    {
        _targetPlayer = inPlayer;
        _inputComponent = _targetPlayer.GetComponent<PlayerInputComponent>();
    }
}
