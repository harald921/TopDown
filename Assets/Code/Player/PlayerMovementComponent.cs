using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : Photon.MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;

    [Header("Speed")]
    [SerializeField] float _moveSpeed    = 2.5f;
    [SerializeField] float _rotationTime = 0.05f;

    [Header("Movement Slow - Applied At Start")]
    [SerializeField] float _maxMovementSlowFactor = 0.5f;
    [SerializeField] float _slowFalloffExponent   = 2.0f;
    [SerializeField] float _slowFalloffSpeed      = 1.0f;

    float _currentRotationVelocity;

    MovementSlower _movementSlower;

    Player _player;
    PlayerInputComponent _inputComponent;
    PlayerFlagComponent _flagComponent;

    public void ManualAwake()
    {
        _player         = GetComponent<Player>();
        _inputComponent = _player.inputComponent;
        _flagComponent  = _player.flagComponent;

        if (!photonView.isMine)
            return;

        _movementSlower = new MovementSlower(_player, _maxMovementSlowFactor, _slowFalloffExponent, _slowFalloffSpeed);
    }

    public void ManualUpdate()
    {
        if (_flagComponent.GetFlag(EFlag.Dead))
            return;

        HandleMovement();
        HandleRotation();

        _movementSlower?.ManualUpdate();
    }


    void HandleRotation()
    {
        // Get the current and target angles
        float   targetAngle   = CalculateTargetRotation().eulerAngles.y;
        Vector3 currentAngles = _rotationTransform.eulerAngles;

        // SmoothDamp a value towards the target
        float   smoothAngle   = Mathf.SmoothDampAngle(currentAngles.y, targetAngle, ref _currentRotationVelocity, _rotationTime);

        // Apply it to the Y axis
        Vector3 targetAngles = new Vector3()
        {
            x = currentAngles.x,
            y = smoothAngle,
            z = currentAngles.z
        };

        // Set the rotation
        _rotationTransform.rotation = Quaternion.Euler(targetAngles);
    }
     
    void HandleMovement()
    {
        float finalMoveSpeed = _moveSpeed * _movementSlower.GetMovementSlowMultiplier();
        Vector3 movement = Vector3.ClampMagnitude(_inputComponent.input.movementDirection * finalMoveSpeed, finalMoveSpeed); 

        transform.position += movement * Time.deltaTime;
    }

    Quaternion CalculateTargetRotation()
    {
        return Quaternion.LookRotation(_inputComponent.input.aimTarget - transform.position);
    }


    class MovementSlower
    {
        readonly float _maxMovementSlowFactor;
        readonly float _falloffExponent;
        readonly float _falloffSpeed;

        float _trauma = 0;

        Player _player;
        

        public MovementSlower(Player inPlayer, float inMovementSlowFactor, float inFalloffExponent, float inFalloffSpeed)
        {
            _player                = inPlayer;

            _maxMovementSlowFactor = inMovementSlowFactor;
            _falloffExponent       = inFalloffExponent;
            _falloffSpeed          = inFalloffSpeed;

            SubscribeEvents();
        }

        void SubscribeEvents()
        {
            _player.healthComponent.OnHealthDamage += () => AddTrauma(0.1f);
            _player.healthComponent.OnShieldDamage += () => AddTrauma(0.05f);

            _player.weaponComponent.OnWeaponPickedUp += (Weapon inPickedUpWeapon) => {
                inPickedUpWeapon.OnFire += () => AddTrauma(_player.weaponComponent.heldWeapon.stats.recoilMoveSlow);
            };
        }

        public void ManualUpdate()
        {
            HandleTraumaFalloff();
        }


        void AddTrauma(float inTrauma)
        {
            _trauma += inTrauma;
            _trauma = Mathf.Clamp01(_trauma);
        }

        public float GetMovementSlowMultiplier()
        {
            float weight = Mathf.Pow(_trauma, _falloffExponent);

            return Mathf.Lerp(1.0f, _maxMovementSlowFactor, weight);
        }


        void HandleTraumaFalloff()
        {
            _trauma -= Time.deltaTime * _falloffSpeed;
            _trauma = Mathf.Clamp01(_trauma);
        }
    }
}