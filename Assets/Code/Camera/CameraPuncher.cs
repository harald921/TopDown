using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPuncher : MonoBehaviour
{
    [SerializeField] float _minFieldOfView          = 40;
    [SerializeField] float _punchStrengthMultiplier = 1.0f;
    [SerializeField] int   _falloffExponent         = 2;
    [SerializeField] float _falloffSpeed            = 1.0f;

    float _trauma = 0.0f;
    float _defaultFieldOfView;

    Camera _mainCamera;


    void Awake()
    {
        _mainCamera = Camera.main;
        _defaultFieldOfView = _mainCamera.fieldOfView;
    }

    void Update()
    {
        SetCurrentPunchAmount(CalculatePunchStrength());
        HandleTraumaFalloff();
    }

    public void Initialize(PlayerHealthComponent inHealthComponent)
    {
        inHealthComponent.OnHealthDamage += () => AddTrauma(0.3f);
    }


    public void AddTrauma(float inTrauma)
    {
        _trauma += inTrauma;
        _trauma = Mathf.Clamp01(_trauma);
    }

    float CalculatePunchStrength()
    {
        float punchAmount = Mathf.Pow(_trauma, _falloffExponent);
        punchAmount *= _punchStrengthMultiplier;

        return punchAmount;
    }

    void SetCurrentPunchAmount(float inPunchPower)
    {
        float currentPunchFieldOfView = _defaultFieldOfView - inPunchPower;
        currentPunchFieldOfView = Mathf.Clamp(currentPunchFieldOfView, _minFieldOfView, _defaultFieldOfView);

        _mainCamera.fieldOfView = currentPunchFieldOfView;
    }

    void HandleTraumaFalloff()
    {
        _trauma -= 1.0f * _falloffSpeed * Time.deltaTime;

        _trauma = Mathf.Clamp01(_trauma);
    }
}
