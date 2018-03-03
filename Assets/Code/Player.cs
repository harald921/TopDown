using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class Player : Photon.MonoBehaviour
{
    [SerializeField] Transform _rotationTransform;
    [SerializeField] Transform _handTransform;

    [Header("Health")]
    [SerializeField] float _maxHealth        = 45.0f;
    [SerializeField] float _healthRegenRate  = 9.0f;
    [SerializeField] float _healthRegenDelay = 10.0f;

    [SerializeField] float _maxShield        = 70.0f;
    [SerializeField] float _shieldRegenDelay = 4.25f;
    [SerializeField] float _shieldRegenRate  = 40.0f;

    [Header("Movement")]
    [SerializeField] float _moveSpeed = 2.5f;

    [Header("Rotation")]
    [SerializeField] float _rotationSpeed = 10.0f;

    [Header("Collision")]
    [SerializeField] LayerMask _collidesWith;
    [SerializeField] float _environmentCollisionRange = 1.0f;
    [SerializeField] float _weaponPickupRadius = 3.0f;

    [Header("Team")]
    [SerializeField] int _team = 0;

    float _currentHealth;
    float _currentShield;

    Collider _collider;
    Weapon _heldWeapon;

    CoroutineHandle _healthRegenHandle;
    CoroutineHandle _shieldRegenHandle;

    public int team => _team;

    public delegate void HealthChangeHandler(float inPreviousHealth, float inCurrentHealth);
    public event HealthChangeHandler OnHealthChange;
    public delegate void ShieldChangeHandler(float inPreviousShield, float inCurrentShield);
    public event ShieldChangeHandler OnShieldChange;

    public event Action OnHealthDamage;
    public event Action OnShieldDamage;
    public event Action OnShieldBreak;
    public event Action OnDeath;


    void Awake()
    {
        _currentHealth = _maxHealth;
        _currentShield = _maxShield;

        _collider = GetComponent<Collider>();

        if (!photonView.isMine)
            return;

        FollowCamera.SetTarget(transform);

        GUIManager.instance.shieldBar.Initialize(this, _maxShield);

        SubscribeEvents();
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        if (Input.GetKeyDown(KeyCode.I))
            DealDamage(10);

        HandleMovement();
        HandleRotation();

        HandleShooting();
        HandleWeaponPickup();
    }


    void SubscribeEvents()
    {
        OnDeath += () => { if (_heldWeapon) photonView.RPC("DropWeapon", PhotonTargets.All); };

        OnHealthChange += (float inPreviousHealth, float inCurrentHealth) =>
        {
            if (inPreviousHealth > inCurrentHealth)
                OnHealthDamage?.Invoke();
        };

        OnShieldChange += (float inPreviousShield, float inCurrentShield) =>
        {
            if (inPreviousShield > inCurrentShield)
                OnShieldDamage?.Invoke();
        };

        OnHealthDamage += () => { _healthRegenHandle = Timing.RunCoroutineSingleton(_HandleHealthRegen(), _healthRegenHandle, SingletonBehavior.Overwrite);
                                  _shieldRegenHandle = Timing.RunCoroutineSingleton(_HandleHealthRegen(), _shieldRegenHandle, SingletonBehavior.Overwrite); };

        OnShieldDamage += () => { _shieldRegenHandle = Timing.RunCoroutineSingleton(_HandleShieldRegen(), _shieldRegenHandle, SingletonBehavior.Overwrite); };

        OnHealthDamage += () => { FindObjectOfType<CameraShaker >().AddTrauma(Vector3.one * 0.5f); }; // TODO: Use manager
        OnHealthDamage += () => { FindObjectOfType<CameraPuncher>().AddTrauma(0.5f);               }; // TODO: Use manager
        OnShieldDamage += () => { FindObjectOfType<CameraShaker >().AddTrauma(Vector3.one * 0.4f); }; // TODO: Use manager
    }

    void HandleWeaponPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Weapon nearbyWeapon = GetClosestWeapon();
            if (nearbyWeapon)
            {
                if (_heldWeapon)
                    photonView.RPC("DropWeapon", PhotonTargets.All);

                photonView.RPC("PickupWeapon", PhotonTargets.All, nearbyWeapon.photonView.viewID);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            if (_heldWeapon)
                photonView.RPC("DropWeapon", PhotonTargets.All);
    }

    [PunRPC]
    void PickupWeapon(int inViewID)
    {
        Weapon weaponToPickup = PhotonView.Find(inViewID).GetComponent<Weapon>();
        weaponToPickup.transform.SetParent(_handTransform);
        weaponToPickup.transform.forward = _handTransform.forward;
        weaponToPickup.transform.localPosition = Vector3.zero;

        weaponToPickup.GetComponent<Rigidbody>().isKinematic = true;

        _heldWeapon = weaponToPickup;
    }

    [PunRPC]
    void DropWeapon()
    {
        Rigidbody weaponRigidbody = _heldWeapon.GetComponent<Rigidbody>();
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.AddTorque(_heldWeapon.transform.forward * 4.0f);

        _heldWeapon.transform.SetParent(null);
        _heldWeapon.ReleaseTrigger();
        _heldWeapon = null;
    }

    Weapon GetClosestWeapon()
    {
        Weapon closestWeapon = null;
        float distToClosestWeapon = Mathf.Infinity;
        foreach (Weapon nearbyWeapon in GetNearbyWeapons())
        {
            float distToNearbyWeapon = Vector3.Distance(transform.position, nearbyWeapon.transform.position);
            if (distToNearbyWeapon > distToClosestWeapon)
                continue;

            closestWeapon = nearbyWeapon;
            distToClosestWeapon = distToNearbyWeapon;
        }

        return closestWeapon;
    }

    List<Weapon> GetNearbyWeapons()
    {
        Collider[] hitColliders = CheckCollision(_weaponPickupRadius);

        List<Weapon> nearbyWeapons = new List<Weapon>();
        foreach (Collider hitCollider in hitColliders)
        {
            Weapon hitWeapon = hitCollider.GetComponent<Weapon>();
            if (hitWeapon)
            {
                if (!hitWeapon.transform.parent)
                    nearbyWeapons.Add(hitWeapon);

                else if (hitWeapon.transform.parent.GetComponent<WeaponSpawner>())
                    nearbyWeapons.Add(hitWeapon);
            }
        }

        return nearbyWeapons;
    }


    void HandleShooting()
    {
        if (!_heldWeapon)
            return;

        if (Input.GetMouseButton(0))
            _heldWeapon.PullTrigger();

        if (Input.GetMouseButtonUp(0))
            _heldWeapon.ReleaseTrigger();
    }


    void HandleRotation()
    {
        _rotationTransform.rotation = Quaternion.RotateTowards(_rotationTransform.rotation, CalculateTargetRotation(), _rotationSpeed * Time.deltaTime);
    }

    Quaternion CalculateTargetRotation()
    {
        return Quaternion.LookRotation(GetAimTarget() - transform.position);
    }

    Vector3 GetAimTarget()
    {
        Vector3 aimTarget = Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, transform.localScale.y, aimTarget.z);

        return aimTarget;
    }

    void HandleMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveSpeed * Time.deltaTime;
        transform.position += movement;

        ResolveCollision(CheckCollision(_environmentCollisionRange));
    }

    Collider[] CheckCollision(float inRange)
    {
        return Physics.OverlapSphere(transform.position - Vector3.up, inRange);
    }

    void ResolveCollision(Collider[] hitColliders)
    {
        foreach (Collider hitCollider in hitColliders)
        {
            if (!_collidesWith.Contains(hitCollider.gameObject.layer))
                continue;

            Vector3 correctionDir;
            float correctionDist;

            // Calculate and perform required movement to resolve collision
            if (Physics.ComputePenetration(_collider, transform.position, transform.rotation, hitCollider, hitCollider.transform.position, hitCollider.transform.rotation, out correctionDir, out correctionDist))
                transform.position += correctionDir * correctionDist;
        }
    }

    [PunRPC]
    void DealDamage(int inDamage)
    {
        // Return if negative damage is recieved
        if (inDamage <= 0)
            return;

        float remainingDamage = inDamage;

        // Shield damage if shield is up
        if (_currentShield > 0)
        {
            float previousShield = _currentShield;
            _currentShield -= remainingDamage;

            if (_currentShield <= 0)
                OnShieldBreak?.Invoke();

            Mathf.Clamp(_currentShield, 0, _maxShield);

            OnShieldChange(previousShield, _currentShield);

            remainingDamage -= previousShield;
        }

        if (remainingDamage <= 0)
            return;

        // Health damage if player is alive
        if (_currentHealth > 0)
        {
            float previousHealth = _currentHealth;
            _currentHealth -= remainingDamage;

            if (_currentHealth <= 0)
                OnDeath?.Invoke();

            Mathf.Clamp(_currentHealth, 0, _maxHealth);

            OnHealthChange(previousHealth, _currentHealth); 
        }
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