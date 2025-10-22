using System;
using TMPro;
using UnityEngine;

public class SurvivalManager : MonoBehaviour
{
    #region Health
    [Header("Health")]
    public float _currentHealth;
    [SerializeField] public float _maxHealth = 100f;
    [SerializeField] private float _healthRegenRate = 5f;
    public float HealthPercent => _currentHealth / _maxHealth;
    #endregion

    #region Hunger
    [Header("Hunger")]
    public float _currentHunger;
    [SerializeField] public float _maxHunger = 100f;
    private float hungerDepleteTimer = 0f;
    [SerializeField] private float hungerDepleteInterval = 2.5f;
    [SerializeField] private float hungerDepleteAmount = 1f;
    public float HungerPercent => _currentHunger / _maxHunger;
    #endregion

    #region Thirst
    [Header("Thirst")]
    public float _currentThirst;
    [SerializeField] public float _maxThirst = 100f;
    private float thirstDepleteTimer = 0f;
    [SerializeField] private float thirstDepleteInterval = 5f;
    [SerializeField] private float thirstDepleteAmount = 1f;
    public float ThirstPercent => _currentThirst / _maxThirst;
    #endregion

    #region Cold
    [Header("Cold")]
    public float _currentCold;
    [SerializeField] public float _maxCold = 100f;
    [SerializeField] private float _coldDepletionRate = 1f;
    [SerializeField] private float _coldReplenishRate = 2f;
    private bool _inWarmZone = false;
    public float ColdPercent => _currentCold / _maxCold;
    #endregion

    #region Fatigue
    [Header("Fatigue")]
    public float _currentFatigue;
    [SerializeField] public float _maxFatigue = 100f;
    [SerializeField] private float _fatigueFromStaminaUsage = 0.01f;
    private float fatigueDepleteTimer = 0f;
    [SerializeField] private float fatigueDepleteInterval = 3f;
    [SerializeField] private float fatigueDepleteAmount = 1f;
    public float FatiguePercent => _currentFatigue / _maxFatigue;
    #endregion

    #region Stamina
    [Header("Stamina")]
    [SerializeField] public float _baseMaxStamina = 4f;
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 5f;
    public float _currentStamina;
    private float _currentStaminaDelayCounter;
    private bool isSprinting;
    public float MaxStamina => Mathf.Lerp(1f, _baseMaxStamina, _currentFatigue / _maxFatigue);
    public float StaminaPercent => _currentStamina / MaxStamina;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showDebugText = true;
    #endregion

    #region Death
    private bool _playerHasDied = false;
    #endregion

    #region Critical Warnings
    public bool IsHungerCritical => HungerPercent <= 0.25f;
    public bool IsThirstCritical => ThirstPercent <= 0.25f;
    public bool IsColdCritical => ColdPercent <= 0.25f;
    public bool IsFatigueCritical => FatiguePercent <= 0.25f;
    public bool IsHealthCritical => HealthPercent <= 0.25f;
    public bool IsStaminaCritical => StaminaPercent <= 0.25f;
    #endregion

    private bool _hasShivered = false;

    private void Start()
    {
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentStamina = _baseMaxStamina;
        _currentFatigue = _maxFatigue;
        _currentCold = _maxCold;
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        UpdateAllStats();
        UpdateStamina();
        UpdateCold();
        UpdateHealth();
        UpdateDebugText();
    }

    // --- Combined Stat Update ---
    private void UpdateAllStats()
    {
        UpdateStat(ref _currentHunger, hungerDepleteAmount, hungerDepleteInterval, ref hungerDepleteTimer, 0, _maxHunger);
        UpdateStat(ref _currentThirst, thirstDepleteAmount, thirstDepleteInterval, ref thirstDepleteTimer, 0, _maxThirst);
        UpdateStat(ref _currentFatigue, fatigueDepleteAmount, fatigueDepleteInterval, ref fatigueDepleteTimer, 0, _maxFatigue);
    }

    private void UpdateStat(ref float stat, float depleteAmount, float depleteInterval, ref float timer, float min, float max)
    {
        timer += Time.deltaTime;
        if (timer >= depleteInterval)
        {
            stat = Mathf.Clamp(stat - depleteAmount, min, max);
            timer = 0f;
        }
    }

    // --- Combined Add Stat ---
    private void AddStat(ref float stat, float amount, float min, float max)
    {
        stat = Mathf.Clamp(stat + amount, min, max);
    }

    public void AddHealth(float amount) => AddStat(ref _currentHealth, amount, 0, _maxHealth);
    public void AddHunger(float amount) => AddStat(ref _currentHunger, amount, 0, _maxHunger);
    public void AddThirst(float amount) => AddStat(ref _currentThirst, amount, 0, _maxThirst);
    public void AddFatigue(float amount) => AddStat(ref _currentFatigue, amount, 0, _maxFatigue);
    public void AddCold(float amount) => AddStat(ref _currentCold, amount, 0, _maxCold);

    // --- Stamina ---
    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }
    public bool HasStamina()
    {
        return _currentStamina > 0;
    }

    private void UpdateStamina() 
    {
        if (isSprinting && HasStamina())
        {
            _currentStamina = Mathf.Max(_currentStamina - _staminaDepletionRate * Time.deltaTime, 0);
            _currentStaminaDelayCounter = 0;
            _currentFatigue = Mathf.Max(_currentFatigue - _fatigueFromStaminaUsage * Time.deltaTime, 0);
        }
        else if (_currentStamina < MaxStamina) // Only recharge if not full
        {
            _currentStaminaDelayCounter += Time.deltaTime;
            if (_currentStaminaDelayCounter >= _staminaRechargeDelay)
            {
                _currentStamina += _staminaRechargeRate * Time.deltaTime;
                _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
            }
        }
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
    }

    // --- Cold ---
    private void UpdateCold()
    {
        if (_inWarmZone)
        {
            _currentCold = Mathf.Min(_currentCold + _coldReplenishRate * Time.deltaTime, _maxCold);
        }
        else
        {
            _currentCold = Mathf.Max(_currentCold - _coldDepletionRate * Time.deltaTime, 0);
        }

        if (ColdPercent < 0.5f && !_hasShivered)
        {
            SoundManager.PlaySound(SoundType.SHIVER);
            _hasShivered = true;
        }
        else if (ColdPercent >= 0.5f)
        {
            _hasShivered = false;
        }
    }

    public void UpdateColdReplenishment(bool isNearFire, bool underShelter)
    {
        _inWarmZone = isNearFire || underShelter;
    }

    // --- Health ---
    private void UpdateHealth()
    {
        if (GetDepletedStatsCount() > 0)
        {
            ApplyHealthDepletion();
        }
        else
        {
            RegenerateHealth();
        }
    }

    private int GetDepletedStatsCount()
    {
        int depletedStatsCount = 0;
        if (_currentHunger <= 0) depletedStatsCount++;
        if (_currentThirst <= 0) depletedStatsCount++;
        if (_currentCold <= 0) depletedStatsCount++;
        if (_currentFatigue <= 0) depletedStatsCount++;
        return depletedStatsCount;
    }

    private void ApplyHealthDepletion()
    {
        float baseHealthDepletionRate = 0.2f;
        float additionalRatePerDepletedStat = 0.2f;
        int depletedStatsCount = GetDepletedStatsCount();
        float healthDepletionRate = baseHealthDepletionRate + (additionalRatePerDepletedStat * (depletedStatsCount - 1));
        _currentHealth = Mathf.Max(_currentHealth - healthDepletionRate * Time.deltaTime, 0);

        if (_currentHealth <= 0)
        {
            HandlePlayerDeath();
        }
    }

    public void RegenerateHealth()
    {
        if (_currentHealth < _maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + _healthRegenRate * Time.deltaTime, _maxHealth);
        }
    }

    // --- Debug ---
    private void UpdateDebugText()
    {
        if (debugText != null && showDebugText)
        {
            debugText.text = $"Hunger: {_currentHunger:F1}\n" +
                             $"Thirst: {_currentThirst:F1}\n" +
                             $"Cold: {_currentCold:F1}\n" +
                             $"Fatigue: {_currentFatigue:F1}\n" +
                             $"Health: {_currentHealth:F1}\n" +
                             $"Stamina: {_currentStamina:F1} / {MaxStamina:F1}";
        }
    }

    // --- Death ---
    private void HandlePlayerDeath()
    {
        if (!_playerHasDied)
        {
            Debug.Log("Player has died.");
            _playerHasDied = true;
            // Game over menu.
        }
    }
}
