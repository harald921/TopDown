using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerHealthComponent : Photon.MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float _maxHealth                     = 45.0f;
    [SerializeField] float _healthRegenDelay              = 10.0f;
    [SerializeField] float _healthRegenRate               = 9.0f;

    [Header("Shield")]
    [SerializeField] float _maxShield                     = 70.0f;
    [SerializeField] float _shieldRegenDelay              = 4.25f;
    [SerializeField] float _shieldRegenRate               = 40.0f;
    [SerializeField] float _shieldBallisticDamageModifier = 0.7f;

    float _currentHealth;
    float currentHealth
    {
        get { return _currentHealth; }
        set
        {
            float previousHealth = _currentHealth;
            _currentHealth = value;

            OnHealthChange?.Invoke(previousHealth, _currentHealth); 
        }
    }

    float _currentShield;
    float currentShield
    {
        get { return _currentShield; }
        set
        {
            float previousShield = _currentShield;
            _currentShield = value;

            OnShieldChange?.Invoke(previousShield, _currentShield);
        }
    }

    CoroutineHandle _healthRegenHandle;
    CoroutineHandle _shieldRegenHandle;

    public delegate void HealthChangeHandler(float inPreviousHealth, float inCurrentHealth);
    public event HealthChangeHandler OnHealthChange;
    public delegate void ShieldChangeHandler(float inPreviousShield, float inCurrentShield);
    public event ShieldChangeHandler OnShieldChange;

    PlayerRespawnComponent _respawnComponent;

    public event Action OnHealthDamage;
    public event Action OnShieldDamage;
    public event Action OnShieldBreak;
    public event Action OnDeath;
    

    void Awake()
    {
        _respawnComponent = GetComponent<PlayerRespawnComponent>();

        SubscribeEvents();

        RefreshHealthAndShield();

        if (!photonView.isMine)
            return;

        FindObjectOfType<ShieldBar>().Initialize(this, _maxShield);
        FindObjectOfType<CameraBloodEffect>().Initialize(this, _maxHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            DealDamage(20);
    }

    void SubscribeEvents()
    {
        // Calling of OnHealthDamage and OnShieldDamage
        OnHealthChange += (float inPreviousHealth, float inCurrentHealth) => {
            if (inPreviousHealth > inCurrentHealth)
                OnHealthDamage?.Invoke();
        };

        OnShieldChange += (float inPreviousShield, float inCurrentShield) => {
            if (inPreviousShield > inCurrentShield)
                OnShieldDamage?.Invoke();
        };

        // Health and Shield regen
        OnHealthDamage += () => {
            _healthRegenHandle = Timing.RunCoroutineSingleton(_HandleHealthRegen(), _healthRegenHandle, SingletonBehavior.Overwrite);
            _shieldRegenHandle = Timing.RunCoroutineSingleton(_HandleShieldRegen(), _shieldRegenHandle, SingletonBehavior.Overwrite);
        };

        OnShieldDamage += () => { _shieldRegenHandle = Timing.RunCoroutineSingleton(_HandleShieldRegen(), _shieldRegenHandle, SingletonBehavior.Overwrite); };

        // Death and Respawn
        OnDeath += () => {
            Timing.KillCoroutines(_shieldRegenHandle);
            Timing.KillCoroutines(_healthRegenHandle);
        };

        _respawnComponent.OnRespawn += RefreshHealthAndShield;
    }

    [PunRPC]
    void DealDamage(int inDamage, Weapon.Type inDamageType = Weapon.Type.None)
    {
        // Return if negative damage is recieved
        if (inDamage <= 0)
            return;

        float remainingDamage = inDamage;

        // Shield damage if shield is up
        if (_currentShield > 0)
        {
            if (inDamageType == Weapon.Type.Ballistic)
                remainingDamage *= _shieldBallisticDamageModifier;

            float previousShield = currentShield;
            currentShield -= remainingDamage;

            if (_currentShield <= 0)
                OnShieldBreak?.Invoke();

            remainingDamage -= previousShield;
        }

        if (remainingDamage <= 0)
            return;

        // Health damage if player is alive
        if (_currentHealth > 0)
        {
            float previousHealth = _currentHealth;
            _currentHealth -= remainingDamage;

            OnHealthChange(previousHealth, _currentHealth);

            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }
    }

    void RefreshHealthAndShield()
    {
        currentHealth = _maxHealth;
        currentShield = _maxShield;
    }

    IEnumerator<float> _HandleHealthRegen()
    {
        yield return Timing.WaitForSeconds(_healthRegenDelay);

        while (_currentHealth < _maxHealth)
        {
            float previousHealth = _currentHealth;
            _currentHealth += _healthRegenRate * Time.deltaTime;

            OnHealthChange?.Invoke(previousHealth, _currentHealth);

            yield return Timing.WaitForOneFrame;
        }

        _currentHealth = _maxHealth;
    }

    IEnumerator<float> _HandleShieldRegen()
    {
        yield return Timing.WaitForSeconds(_shieldRegenDelay);

        while (_currentShield < _maxShield)
        {
            float previousShield = _currentShield;
            _currentShield += _shieldRegenRate * Time.deltaTime;

            OnShieldChange?.Invoke(previousShield, _currentShield);

            yield return Timing.WaitForOneFrame;
        }

        _currentShield = _maxShield;
    }
}