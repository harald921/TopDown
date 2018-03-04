using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] Gradient _colorGradient;

    [Header("References")]
    [SerializeField] Image _fill;

    float _maxShield;


    public void Initialize(PlayerHealthComponent inPlayerHealthComponent, float inMaxShield)
    {
        inPlayerHealthComponent.OnShieldChange += UpdateBar;
        _maxShield = inMaxShield;

        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }

    void UpdateBar(float inPreviousShield, float inCurrentShield)
    {
        float shieldPercentage = (inCurrentShield / _maxShield);
        _fill.fillAmount = shieldPercentage;
        _fill.color = _colorGradient.Evaluate(shieldPercentage);
    }
}
