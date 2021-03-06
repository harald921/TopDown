﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] Vector3 _maxYawPitchRoll         = new Vector3(2, 2, 2);
    [SerializeField] float   _falloffSpeed            = 2.0f;
    [SerializeField] int     _falloffExponent         = 3;
    [SerializeField] float   _shakeStrengthMultiplier = 1.0f;
    [SerializeField] float   _shakeFrequency          = 13.0f;

    Vector3 _trauma = Vector3.zero;
    Vector3 _defaultRotation;

    Camera _mainCamera;


    void Awake()
    {
        _mainCamera = Camera.main;
        _defaultRotation = transform.eulerAngles;
    }

    void Update()
    {
        _mainCamera.transform.eulerAngles = _defaultRotation + CalculateShakeAngle(CalculateShakeAmount()); 

        HandleTraumaFalloff();
    }

    public void Initialize(PlayerHealthComponent inHealthComponent)
    {
        inHealthComponent.OnHealthDamage += () => AddTrauma(Vector3.one * 0.25f);
        inHealthComponent.OnShieldDamage += () => AddTrauma(Vector3.one * 0.20f);
    }

    Vector3 CalculateShakeAmount()
    {
        Vector3 shakeAmount = new Vector3()
        {
            x = Mathf.Pow(_trauma.x, _falloffExponent),
            y = Mathf.Pow(_trauma.y, _falloffExponent),
            z = Mathf.Pow(_trauma.z, _falloffExponent)
        };

        shakeAmount *= _shakeStrengthMultiplier;

        shakeAmount.x = Mathf.Clamp01(shakeAmount.x);
        shakeAmount.y = Mathf.Clamp01(shakeAmount.y);
        shakeAmount.z = Mathf.Clamp01(shakeAmount.z);

        return shakeAmount;
    }

    Vector3 CalculateShakeAngle(Vector3 inShakeAmount)
    {
        return new Vector3()
        {
            x = _maxYawPitchRoll.x * inShakeAmount.x * GetShakePerlin(0),
            y = _maxYawPitchRoll.y * inShakeAmount.y * GetShakePerlin(1),
            z = _maxYawPitchRoll.z * inShakeAmount.z * GetShakePerlin(2)
        };
    }

    float GetShakePerlin(int inSeed)
    {
        float sampleX, sampleY;

        sampleX = sampleY = Time.time * _shakeFrequency;
        sampleX += inSeed;
        sampleY += inSeed + 3;

        return (Mathf.PerlinNoise(sampleX, sampleY) * 2) - 1;
    }

    void HandleTraumaFalloff()
    {
        _trauma -= Vector3.one * _falloffSpeed * Time.deltaTime;
        _trauma = new Vector3()
        {
            x = Mathf.Clamp01(_trauma.x),
            y = Mathf.Clamp01(_trauma.y),
            z = Mathf.Clamp01(_trauma.z)
        };
    }


    public void AddTrauma(Vector3 inTrauma)
    {
        _trauma += inTrauma;
    }
}
