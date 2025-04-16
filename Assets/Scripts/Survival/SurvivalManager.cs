using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// This script manages the player's survival mechanics, including hunger, thirst, cold, fatigue, and stamina.
// It updates these stats over time and provides methods for replenishment and environmental interactions.
public class SurvivalManager : MonoBehaviour
{

    //[SerializeField] private HealthManager healthManager; // Reference to the health system.
    #region Hunger
    [SerializeField] private float _maxHunger = 100f;
    [SerializeField] private float _hungerDepletionRate = 2f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;
    #endregion
    #region Thirst
    [SerializeField] private float _maxThirst = 100f;
    [SerializeField] private float _thirstDepletionRate = 1f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxThirst;
    #endregion
    #region Cold
    [SerializeField] private float _maxCold = 100f;
    [SerializeField] private float _coldDepletionRate = 1f;
    [SerializeField] private float _coldReplenishRate = 2f;
    private float _currentCold;
    public float ColdPercent => _currentCold / _maxCold;
    #endregion
    #region Fatigue
    [SerializeField] private float _maxFatigue = 100f;
    [SerializeField] private float _fatigueDepletionRate = 1f;
    private float _currentFatigue;
    public float FatiguePercent => _currentFatigue / _maxFatigue;
    #endregion
    #region Stamina
    [SerializeField] private float _maxStamina = 5f;
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 5f;
    private float _currentStamina;
    private float _currentStaminaDelayCounter;
    public float StaminaPercent => _currentStamina / _maxStamina;
    #endregion

    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showDebugText = true;

    public event Action OnHungerDepleted;
    public event Action OnThirstDepleted;
    public event Action OnColdDepleted;


    private bool _isInColdReplenishZone = false;

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
    }

    /// <summary>
    /// Updates the player's survival stats every frame, including hunger, thirst, cold, fatigue, and stamina.
    /// Handles depletion over time and environmental effects.
    /// </summary>
    private void Update()
    {
        _currentHunger = DepleteStat(_currentHunger, _hungerDepletionRate, Time.deltaTime);
        _currentThirst = DepleteStat(_currentThirst, _thirstDepletionRate, Time.deltaTime);
        _currentCold = _isInColdReplenishZone
            ? Mathf.Min(_currentCold + _coldReplenishRate * Time.deltaTime, _maxCold)
            : DepleteStat(_currentCold, _coldDepletionRate, Time.deltaTime);
        _currentFatigue = DepleteStat(_currentFatigue, _fatigueDepletionRate, Time.deltaTime);

        // Handle stamina depletion and recharge.
        if (InputManager.Instance.sprint)
        {
            _currentStamina = DepleteStat(_currentStamina, _staminaDepletionRate, Time.deltaTime);
            _currentStaminaDelayCounter = 0; // Reset recharge delay when sprinting.
        }
        else if (_currentStamina < _maxStamina)
        {
            _currentStaminaDelayCounter += Time.deltaTime;

            if (_currentStaminaDelayCounter >= _staminaRechargeDelay)
            {
                _currentStamina += _staminaRechargeRate * Time.deltaTime;
                _currentStamina = Mathf.Min(_currentStamina, _maxStamina); // Ensure stamina doesn't exceed max.
            }
        }

        if (_currentHunger <= 0)
        {
            _currentHunger = 0;
            OnHungerDepleted?.Invoke(); // Notify listeners that hunger is depleted.
        }
        if (_currentThirst <= 0)
        {
            _currentThirst = 0;
            OnThirstDepleted?.Invoke(); // Notify listeners that thirst is depleted.
        }
        if (_currentCold <= 0)
        {
            _currentCold = 0;
            OnColdDepleted?.Invoke(); // Notify listeners that cold has depleted.
        }

        if (debugText != null && showDebugText)
        {
            debugText.text = $"Hunger: {_currentHunger:F1}\n" +
                             $"Thirst: {_currentThirst:F1}\n" +
                             $"Cold: {_currentCold:F1}\n" +
                             $"Fatigue: {_currentFatigue:F1}\n" +
                             $"Stamina: {_currentStamina:F1}";
        }
        else if (debugText != null)
        {
            debugText.text = ""; // Clear the text if debugging is disabled.
        }

    }

    private float DepleteStat(float currentValue, float depletionRate, float deltaTime, float minValue = 0)
    {
        currentValue -= depletionRate * deltaTime;
        return Mathf.Max(currentValue, minValue); // Ensure the value doesn't go below the minimum.
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

    public void ResetStats()
    {
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentCold = _maxCold;
        _currentFatigue = _maxFatigue;
        _currentStamina = _maxStamina;
        _currentStaminaDelayCounter = 0;
    }

    
}
