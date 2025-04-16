using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// This script manages the player's survival mechanics, including hunger, thirst, cold, fatigue, and stamina.
// It updates these stats over time and provides methods for replenishment and environmental interactions.
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
    public float ColdPercent => _currentCold / _maxCold;
    #endregion

    #region Fatigue
    [Header("Fatigue")]
    [SerializeField] private float _maxFatigue = 100f;
    [SerializeField] private float _fatigueDepletionRate = 1f;
    private float _currentFatigue;
    public float FatiguePercent => _currentFatigue / _maxFatigue;
    #endregion

    #region Stamina
    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 5f;
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 5f;
    private float _currentStamina;
    private float _currentStaminaDelayCounter;
    public float StaminaPercent => _currentStamina / _maxStamina;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showDebugText = true;
    #endregion

    /*
    public event Action OnHungerDepleted;
    public event Action OnThirstDepleted;
    public event Action OnColdDepleted;
    */


    private bool _isInColdReplenishZone = false;
    private int _previousCriticalStatsCount = -1; // Initialize to -1 to ensure the first log is triggered
    private float _previousHealthDepletionRate = -1f; // Initialize to -1 to ensure the first log is triggered


    /// <summary>
    /// Initializes the player's survival stats to their maximum values at the start of the game.
    /// </summary>
    private void Start()
    {
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentStamina = _maxStamina;
        _currentCold = _maxCold;
        _currentFatigue = _maxFatigue;
        _currentHealth = _maxHealth;
    }

    /// <summary>
    /// Updates the player's survival stats every frame, including hunger, thirst, cold, fatigue, and stamina.
    /// Handles depletion over time and environmental effects.
    /// </summary>
    private void Update()
    {
        // Update survival stats
        _currentHunger = DepleteStat(_currentHunger, _hungerDepletionRate, Time.deltaTime);
        _currentThirst = DepleteStat(_currentThirst, _thirstDepletionRate, Time.deltaTime);
        _currentCold = _isInColdReplenishZone
            ? Mathf.Min(_currentCold + _coldReplenishRate * Time.deltaTime, _maxCold)
            : DepleteStat(_currentCold, _coldDepletionRate, Time.deltaTime);
        _currentFatigue = DepleteStat(_currentFatigue, _fatigueDepletionRate, Time.deltaTime);

        // Handle stamina depletion and recharge
        if (InputManager.Instance.sprint)
        {
            _currentStamina = DepleteStat(_currentStamina, _staminaDepletionRate, Time.deltaTime);
            _currentStaminaDelayCounter = 0; // Reset recharge delay when sprinting
        }
        else if (_currentStamina < _maxStamina)
        {
            _currentStaminaDelayCounter += Time.deltaTime;

            if (_currentStaminaDelayCounter >= _staminaRechargeDelay)
            {
                _currentStamina += _staminaRechargeRate * Time.deltaTime;
                _currentStamina = Mathf.Min(_currentStamina, _maxStamina); // Ensure stamina doesn't exceed max
            }
        }

        // Apply health depletion or regeneration
        if (GetCriticalStatsCount() > 0)
        {
            ApplyHealthDepletion();
        }
        else
        {
            RegenerateHealth();
        }

        // Update debug text
        if (debugText != null && showDebugText)
        {
            debugText.text = $"Hunger: {_currentHunger:F1}\n" +
                             $"Thirst: {_currentThirst:F1}\n" +
                             $"Cold: {_currentCold:F1}\n" +
                             $"Fatigue: {_currentFatigue:F1}\n" +
                             $"Health: {_currentHealth:F1}\n" +
                             $"Stamina: {_currentStamina:F1}";
        }
        else if (debugText != null)
        {
            debugText.text = ""; // Clear the text if debugging is disabled
        }
    }


    /// <summary>
    /// Counts the number of survival stats that are in a critical state.
    /// </summary>
    /// <returns>The number of critical stats.</returns>
    private int GetCriticalStatsCount()
    {
        int criticalStatsCount = 0;
        if (_currentHunger <= 0) criticalStatsCount++;
        if (_currentThirst <= 0) criticalStatsCount++;
        if (_currentCold <= 0) criticalStatsCount++;
        if (_currentFatigue <= 0) criticalStatsCount++;
        return criticalStatsCount;
    }


    /// <summary>
    /// Depletes a stat over time based on the specified depletion rate.
    /// Ensures the stat does not fall below the specified minimum value.
    /// </summary>
    /// <param name="currentValue">The current value of the stat.</param>
    /// <param name="depletionRate">The rate at which the stat depletes.</param>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    /// <param name="minValue">The minimum value the stat can reach (default is 0).</param>
    /// <returns>The updated stat value after depletion.</returns>
    private float DepleteStat(float currentValue, float depletionRate, float deltaTime, float minValue = 0)
    {
        currentValue -= depletionRate * deltaTime;
        return Mathf.Max(currentValue, minValue); // Ensure the value doesn't go below the minimum.
    }


    /// <summary>
    /// Reduces the player's health.
    /// The health depletion rate increases with the number of critical stats.
    /// </summary>
    private void ApplyHealthDepletion()
    {
        float baseHealthDepletionRate = 5f; // Base rate for a single critical stat
        float additionalRatePerCriticalStat = 1f; // Additional rate for each extra critical stat

        int criticalStatsCount = GetCriticalStatsCount();

        // Calculate the total health depletion rate
        float healthDepletionRate = baseHealthDepletionRate + (additionalRatePerCriticalStat * (criticalStatsCount - 1));
        _currentHealth = Mathf.Max(_currentHealth - healthDepletionRate * Time.deltaTime, 0);

        // debugging 
        if (criticalStatsCount != _previousCriticalStatsCount || !Mathf.Approximately(healthDepletionRate, _previousHealthDepletionRate))
        {
            Debug.Log($"Critical Stats: {criticalStatsCount}, Health Depletion Rate: {healthDepletionRate:F2}, Current Health: {_currentHealth:F2}");
        }

        // Update
        _previousCriticalStatsCount = criticalStatsCount;
        _previousHealthDepletionRate = healthDepletionRate;

        // Check for player death
        if (_currentHealth <= 0)
        {
            HandlePlayerDeath();
        }
    }


    /// <summary>
    /// Replenishes the player's hunger and thirst by the specified amounts.
    /// </summary>
    /// <param name="hungerAmount">The amount to replenish hunger.</param>
    /// <param name="thirstAmount">The amount to replenish thirst.</param>
    public void ReplenishHungerThirst(float hungerAmount, float thirstAmount)
    {
        _currentHunger += hungerAmount;
        _currentThirst += thirstAmount;
        if (_currentHunger > _maxHunger) _currentHunger = _maxHunger;
        if (_currentThirst > _maxThirst) _currentThirst = _maxThirst;
    }


    /// <summary>
    /// Updates the cold replenishment state based on environmental factors.
    /// </summary>
    /// <param name="isNearFire">True if the player is near a fire.</param>
    /// <param name="isWearingWarmClothing">True if the player is wearing warm clothing.</param>
    public void UpdateColdReplenishment(bool isNearFire, bool isWearingWarmClothing)
    {
        _isInColdReplenishZone = isNearFire || isWearingWarmClothing;
    }


    /// <summary>
    /// Checks whether the player has any stamina remaining.
    /// </summary>
    /// <returns>True if the player has stamina, false otherwise.</returns>
    public bool HasStamina()
    {
        return _currentStamina > 0;
    }


    /// <summary>
    /// Regenerates player's health if stats are above zero.
    /// </summary>
    private void RegenerateHealth()
    {
        if (_currentHealth < _maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + _healthRegenRate * Time.deltaTime, _maxHealth);
        }
    }


    /// <summary>
    /// Saves or loads the player's survival stats to/from persistent storage.
    /// </summary>
    /// <param name="isSaving">True to save stats, false to load stats.</param>
    private void SaveOrLoadStats(bool isSaving)
    {
        if (isSaving)
        {
            PlayerPrefs.SetFloat("Hunger", _currentHunger);
            PlayerPrefs.SetFloat("Thirst", _currentThirst);
            PlayerPrefs.SetFloat("Cold", _currentCold);
            PlayerPrefs.SetFloat("Fatigue", _currentFatigue);
            PlayerPrefs.SetFloat("Stamina", _currentStamina);
        }
        else
        {
            _currentHunger = PlayerPrefs.GetFloat("Hunger", _maxHunger);
            _currentThirst = PlayerPrefs.GetFloat("Thirst", _maxThirst);
            _currentCold = PlayerPrefs.GetFloat("Cold", _maxCold);
            _currentFatigue = PlayerPrefs.GetFloat("Fatigue", _maxFatigue);
            _currentStamina = PlayerPrefs.GetFloat("Stamina", _maxStamina);
        }
    }

    public void SaveStats() => SaveOrLoadStats(true);
    public void LoadStats() => SaveOrLoadStats(false);


    /// <summary>
    /// Resets all survival stats to their maximum values.
    /// </summary>
    public void ResetStats()
    {
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentCold = _maxCold;
        _currentFatigue = _maxFatigue;
        _currentStamina = _maxStamina;
        _currentStaminaDelayCounter = 0;
    }


    /// <summary>
    /// Handles the player's death by triggering the appropriate logic.
    /// </summary>
    private void HandlePlayerDeath()
    {
        // Example: gameManager.TriggerGameOver();
        Debug.Log("Player has died.");
    }


}

