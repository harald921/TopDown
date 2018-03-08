using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> _renderersToFlash;
    [SerializeField] Color _color     = Color.red;
    [SerializeField] float _fadeSpeed = 0.5f;
    
    float _trauma = 0;

    Dictionary<MeshRenderer, Color> _defaultColors = new Dictionary<MeshRenderer, Color>();


    void Awake()
    {
        GetComponent<PlayerHealthComponent>().OnHealthDamage += () => { AddTrauma(1); };

        foreach (MeshRenderer renderer in _renderersToFlash)
            _defaultColors.Add(renderer, renderer.material.color);
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

        foreach (MeshRenderer renderer in _renderersToFlash)
            renderer.material.color = Color.Lerp(_defaultColors[renderer], _color, _trauma);

        _trauma -= Time.deltaTime * _fadeSpeed;
    }
}
