using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] Gradient _colorGradient;
    [SerializeField] Image _fill;

    float _maxShield;


    public void Initialize(PlayerHealthComponent inPlayerHealthComponent, float inMaxShield)
    {
        _maxShield = inMaxShield;

        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        inPlayerHealthComponent.OnShieldChange += UpdateBar;
    }

    void UpdateBar(PlayerHealthComponent.ShieldChangeArgs inArgs)
    {
        float shieldPercentage = (inArgs.inCurrentShield / _maxShield);
        _fill.fillAmount = shieldPercentage;
        _fill.color = _colorGradient.Evaluate(shieldPercentage);
    }
}
