using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPuncher : MonoBehaviour
{
    static CameraPuncher _instance;
    public static CameraPuncher instance;

    [SerializeField] float _minFieldOfView          = 40;
    [SerializeField] float _punchStrengthMultiplier = 1.0f;
    [SerializeField] int   _falloffExponent         = 2;
    [SerializeField] float _falloffSpeed            = 1.0f;

    float _trauma = 0.0f;
    float _defaultFieldOfView;

    Camera _mainCamera;


    void Awake()
    {
        _instance = this;
        _mainCamera = Camera.main;
        _defaultFieldOfView = _mainCamera.fieldOfView;
    }


    void Update()
    {
        SetCurrentPunchAmount(CalculatePunchStrength());
        HandleTraumaFalloff();
    }


    public void AddTrauma(float inTrauma)
    {
        _trauma += inTrauma;
        _trauma = Mathf.Clamp01(_trauma);
    }


    void SetCurrentPunchAmount(float inPunchPower)
    {
        float currentPunchFieldOfView = _defaultFieldOfView - inPunchPower;
        currentPunchFieldOfView = Mathf.Clamp(currentPunchFieldOfView, _minFieldOfView, _defaultFieldOfView);

        _mainCamera.fieldOfView = currentPunchFieldOfView;
    }

    float CalculatePunchStrength()
    {
        float punchAmount = Mathf.Pow(_trauma, _falloffExponent);
        punchAmount *= _punchStrengthMultiplier;

        return punchAmount;
    }

    void HandleTraumaFalloff()
    {
        _trauma -= 1.0f * _falloffSpeed * Time.deltaTime;

        _trauma = Mathf.Clamp01(_trauma);
    }
}
