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

            OnHealthChange?.Invoke(new HealthChangeArgs() { inCurrentHealth = _currentHealth, inPreviousHealth = previousHealth });
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

            OnShieldChange?.Invoke(new ShieldChangeArgs() { inCurrentShield = _currentShield, inPreviousShield = previousShield });
        }
    }

    CoroutineHandle _healthRegenHandle;
    CoroutineHandle _shieldRegenHandle;

    PlayerRespawnComponent _respawnComponent;

    public class HealthChangeArgs
    {
        public float inCurrentHealth;
        public float inPreviousHealth;
    }
    public event Action<HealthChangeArgs> OnHealthChange;

    public class ShieldChangeArgs
    {
        public float inCurrentShield;
        public float inPreviousShield;
    }
    public event Action<ShieldChangeArgs> OnShieldChange;

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
            DealDamage(5);
    }

    void SubscribeEvents()
    {
        // Calling of OnHealthDamage and OnShieldDamage
        OnHealthChange += args => {
            if (args.inPreviousHealth > args.inCurrentHealth)
                OnHealthDamage?.Invoke();
        };

        OnShieldChange += args => {
            if (args.inPreviousShield > args.inCurrentShield)
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

        _respawnComponent.OnSpawn += RefreshHealthAndShield;
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
            currentHealth -= remainingDamage;

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

        if (_currentHealth < 0)
            currentHealth = 0;

        while (_currentHealth < _maxHealth)
        {
            currentHealth = Mathf.Clamp(_healthRegenRate * Time.deltaTime, 0, _maxHealth);
            yield return Timing.WaitForOneFrame;
        }

        currentHealth = _maxHealth;
    }

    IEnumerator<float> _HandleShieldRegen()
    {
        yield return Timing.WaitForSeconds(_shieldRegenDelay);

        if (_currentHealth < 0)
            currentShield = 0;

        while (_currentShield < _maxShield)
        {
            currentShield = Mathf.Clamp(_currentShield + _shieldRegenRate * Time.deltaTime, 0, _maxShield);
            yield return Timing.WaitForOneFrame;
        }

        currentShield = _maxShield;
    }
}