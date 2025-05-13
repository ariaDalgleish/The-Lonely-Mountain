using System;
using TMPro;
using UnityEngine;

public class SurvivalManager : MonoBehaviour
{
    #region Health
    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _healthRegenRate = 5f;
    private float _currentHealth;
    public float HealthPercent => _currentHealth / _maxHealth;
    #endregion

    #region Hunger
    [Header("Hunger")]
    [SerializeField] private float _maxHunger = 100f;
    [SerializeField] private float _hungerDepletionRate = 2f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;
    #endregion

    #region Thirst
    [Header("Thirst")]
    [SerializeField] private float _maxThirst = 100f;
    [SerializeField] private float _thirstDepletionRate = 1f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxThirst;
    #endregion

    #region Cold
    [Header("Cold")]
    [SerializeField] private float _maxCold = 100f;
    [SerializeField] private float _coldDepletionRate = 1f;
    [SerializeField] private float _coldReplenishRate = 2f;
    private float _currentCold;
    private bool _inWarmZone = false; // Tracks if the player is in a warm zone
    public float ColdPercent => _currentCold / _maxCold;
    #endregion

    #region Fatigue
    [Header("Fatigue")]
    [SerializeField] private float _maxFatigue = 100f;
    [SerializeField] private float _fatigueDepletionRate = 1f;
    [SerializeField] private float _fatigueFromStaminaUsage = 0.5f; // Fatigue penalty for stamina usage
    private float _currentFatigue;
    public float FatiguePercent => _currentFatigue / _maxFatigue;
    #endregion

    #region Stamina
    [Header("Stamina")]
    [SerializeField] private float _baseMaxStamina = 4f; // Base max stamina
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 5f;
    private float _currentStamina;
    private float _currentStaminaDelayCounter;

    // Dynamically calculated max stamina based on fatigue
    public float MaxStamina => Mathf.Lerp(1f, _baseMaxStamina, _currentFatigue / _maxFatigue);
    public float StaminaPercent => _currentStamina / MaxStamina;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showDebugText = true;
    #endregion

    private void Start()
    {
        #region Initialization
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentStamina = _baseMaxStamina;
        _currentFatigue = _maxFatigue;
        _currentCold = _maxCold;
        _currentHealth = _maxHealth;
        #endregion
    }

    private void Update()
    {
        #region Update Stats
        UpdateStats();
        UpdateStamina();
        UpdateCold();
        UpdateHealth();
        UpdateDebugText();
        #endregion
    }

    private void UpdateStats()
    {
        // Deplete fatigue and hunger over time
        #region Depletion
        _currentFatigue = DepleteStat(_currentFatigue, _fatigueDepletionRate, Time.deltaTime);
        _currentHunger = DepleteStat(_currentHunger, _hungerDepletionRate, Time.deltaTime);
        _currentThirst = DepleteStat(_currentThirst, _thirstDepletionRate, Time.deltaTime);
        #endregion
    }

    public bool HasStamina()
    {
        return _currentStamina > 0;
    }

    private void UpdateStamina()
    {
        if (InputManager.Instance.sprint && HasStamina())
        {
            // Deplete stamina when sprinting
            _currentStamina = DepleteStat(_currentStamina, _staminaDepletionRate, Time.deltaTime);
            _currentStaminaDelayCounter = 0;

            // Reduce fatigue as a penalty for stamina usage
            _currentFatigue = DepleteStat(_currentFatigue, _fatigueFromStaminaUsage, Time.deltaTime);
        }
        else if (_currentStamina < MaxStamina)
        {
            // Recharge stamina when not sprinting
            _currentStaminaDelayCounter += Time.deltaTime;

            if (_currentStaminaDelayCounter >= _staminaRechargeDelay)
            {
                _currentStamina += _staminaRechargeRate * Time.deltaTime;
                _currentStamina = Mathf.Min(_currentStamina, MaxStamina); // Clamp to max stamina
            }
        }

        // Clamp stamina to dynamically calculated max stamina
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
    }

    private void UpdateCold()
    {
        #region Cold
        if (_inWarmZone)
        {
            // Replenish cold when in a warm zone
            _currentCold = Mathf.Min(_currentCold + _coldReplenishRate * Time.deltaTime, _maxCold);
        }
        else
        {
            // Deplete cold when not in a warm zone
            _currentCold = DepleteStat(_currentCold, _coldDepletionRate, Time.deltaTime);
        }
        #endregion
    }

    private void UpdateHealth()
    {
        #region Health
        if (GetCriticalStatsCount() > 0)
        {
            ApplyHealthDepletion();
        }
        else
        {
            RegenerateHealth();
        }
        #endregion
    }

    private void UpdateDebugText()
    {
        #region Debug
        if (debugText != null && showDebugText)
        {
            debugText.text = $"Hunger: {_currentHunger:F1}\n" +
                             $"Thirst: {_currentThirst:F1}\n" +
                             $"Cold: {_currentCold:F1}\n" +
                             $"Fatigue: {_currentFatigue:F1}\n" +
                             $"Health: {_currentHealth:F1}\n" +
                             $"Stamina: {_currentStamina:F1} / {MaxStamina:F1}";
        }
        #endregion
    }

    private int GetCriticalStatsCount()
    {
        #region Critical Stats
        int criticalStatsCount = 0;
        if (_currentHunger <= 0) criticalStatsCount++;
        if (_currentThirst <= 0) criticalStatsCount++;
        if (_currentCold <= 0) criticalStatsCount++;
        if (_currentFatigue <= 0) criticalStatsCount++;
        return criticalStatsCount;
        #endregion
    }

    private float DepleteStat(float currentValue, float depletionRate, float deltaTime, float minValue = 0)
    {
        #region Depletion function
        currentValue -= depletionRate * deltaTime;
        return Mathf.Max(currentValue, minValue); // Ensure the value doesn't go below the minimum
        #endregion
    }

    private void ApplyHealthDepletion()
    {
        #region Health Depletion
        float baseHealthDepletionRate = 5f;
        float additionalRatePerCriticalStat = 1f;

        int criticalStatsCount = GetCriticalStatsCount();
        float healthDepletionRate = baseHealthDepletionRate + (additionalRatePerCriticalStat * (criticalStatsCount - 1));
        _currentHealth = Mathf.Max(_currentHealth - healthDepletionRate * Time.deltaTime, 0);

        if (_currentHealth <= 0)
        {
            HandlePlayerDeath();
        }
        #endregion
    }

    private void RegenerateHealth()
    {
        #region Health Regeneration
        if (_currentHealth < _maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + _healthRegenRate * Time.deltaTime, _maxHealth);
        }
        #endregion
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player has died.");
    }

    public void UpdateColdReplenishment(bool isNearFire, bool underShelter)
    {
        _inWarmZone = isNearFire || underShelter;
    }
}
