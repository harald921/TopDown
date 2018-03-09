using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> _renderersToFlash;
    [SerializeField] Color _healthDamageColor     = Color.red;
    [SerializeField] Color _shieldDamageColor     = Color.cyan;
    [SerializeField] float _fadeSpeed = 0.5f;
    
    float _healthTrauma = 0;
    float _shieldTrauma = 0;

    Dictionary<MeshRenderer, Color> _defaultColors = new Dictionary<MeshRenderer, Color>();


    void Awake()
    {
        foreach (MeshRenderer renderer in _renderersToFlash)
            _defaultColors.Add(renderer, renderer.material.color);

        GetComponent<PlayerHealthComponent>().OnHealthDamage += () => { AddHealthTrauma(1); };
        GetComponent<PlayerHealthComponent>().OnShieldDamage += () => { AddShieldTrauma(1); };
    }

    void Update()
    {
        HandleShieldDamageFlash();

        if (_shieldTrauma > 0)
            return;

        HandleHealthDamageFlash();
    }


    void AddHealthTrauma(float inTrauma)
    {
        _healthTrauma += inTrauma;
    }

    void AddShieldTrauma(float inTrauma)
    {
        _shieldTrauma += inTrauma;
    }

    void HandleHealthDamageFlash()
    {
        _healthTrauma = Mathf.Clamp01(_healthTrauma);

        foreach (MeshRenderer renderer in _renderersToFlash)
            renderer.material.color = Color.Lerp(_defaultColors[renderer], _healthDamageColor, _healthTrauma);

        _healthTrauma -= Time.deltaTime * _fadeSpeed;
    }

    void HandleShieldDamageFlash()
    {
        _shieldTrauma = Mathf.Clamp01(_shieldTrauma);

        foreach (MeshRenderer renderer in _renderersToFlash)
            renderer.material.color = Color.Lerp(_defaultColors[renderer], _shieldDamageColor, _shieldTrauma);

        _shieldTrauma -= Time.deltaTime * _fadeSpeed;
    }
}
