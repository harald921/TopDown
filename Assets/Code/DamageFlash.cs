using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] Color _color     = Color.red;
    [SerializeField] float _fadeSpeed = 0.5f;

    float _trauma = 0;

    Material _flashMaterial;
    Color _defaultColor;


    void Awake()
    {
        _flashMaterial = GetComponent<MeshRenderer>().material;
        _defaultColor = _flashMaterial.color;

        GetComponent<Player>().OnHealthDamage += () => { AddTrauma(1); };
    }

    void Update()
    {
        HandleDamageFlash();    
    }


    public void AddTrauma(float inTrauma)
    {
        _trauma += inTrauma;
    }

    void HandleDamageFlash()
    {
        _trauma = Mathf.Clamp01(_trauma);

        _flashMaterial.color = Color.Lerp(_defaultColor, _color, _trauma);

        _trauma -= Time.deltaTime * _fadeSpeed;
    }
}
