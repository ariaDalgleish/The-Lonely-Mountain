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
    public float HealthPercent => _currentHealth / _maxHealth; // Public getter for health percentage
    #endregion

    #region Hunger
    [Header("Hunger")]
    public float _currentHunger;
    [SerializeField] public float _maxHunger = 100f;
    //[SerializeField] private float _hungerDepletionRate = 2f;
    private float hungerDepleteTimer = 0f; // timer to track hunger depletion
    [SerializeField] private float hungerDepleteInterval = 2.5f; // seconds
    [SerializeField] private float hungerDepleteAmount = 1f; // amount to deplete each interval
    public float HungerPercent => _currentHunger / _maxHunger;
    #endregion

    #region Thirst
    [Header("Thirst")]
    public float _currentThirst;
    [SerializeField] public float _maxThirst = 100f;
   // [SerializeField] private float _thirstDepletionRate = 5f;
    private float thirstDepleteTimer = 0f; // timer to track hunger depletion
    [SerializeField] private float thirstDepleteInterval = 5f; // seconds
    [SerializeField] private float thirstDepleteAmount = 1f; // amount to deplete each interval
    public float ThirstPercent => _currentThirst / _maxThirst;
    #endregion

    #region Cold
    [Header("Cold")]
    public float _currentCold;
    [SerializeField] public float _maxCold = 100f;
    [SerializeField] private float _coldDepletionRate = 1f;
    [SerializeField] private float _coldReplenishRate = 2f;
    private bool _inWarmZone = false; // Tracks if the player is in a warm zone
    public float ColdPercent => _currentCold / _maxCold; // Public getter for cold percentage
    #endregion

    #region Fatigue
    [Header("Fatigue")]
    public float _currentFatigue;
    [SerializeField] public float _maxFatigue = 100f;
   // [SerializeField] private float _fatigueDepletionRate = 1f;
    [SerializeField] private float _fatigueFromStaminaUsage = 0.5f; // Fatigue penalty for stamina usage
  
    private float fatigueDepleteTimer = 0f; // timer to track hunger depletion
    [SerializeField] private float fatigueDepleteInterval = 3f; // seconds
    [SerializeField] private float fatigueDepleteAmount = 1f; // amount to deplete each interval
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

    #region Death
    private bool _playerHasDied = false;
    #endregion
    
    private bool _hasShivered = false;

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
    private void UpdateStats() // Update hunger, thirst, and fatigue over time
    {
        #region Deplete Stats Over Time
        
        hungerDepleteTimer += Time.deltaTime; // Increment timer
        if (hungerDepleteTimer >= hungerDepleteInterval) // Check if it's time to deplete hunger
        {
            _currentHunger = Mathf.Max(_currentHunger - hungerDepleteAmount, 0); // Deplete hunger and clamp to 0 
            hungerDepleteTimer = 0f; // Reset timer
        }

        thirstDepleteTimer += Time.deltaTime;
        if (thirstDepleteTimer >= thirstDepleteInterval)
        {
            _currentThirst = Mathf.Max(_currentThirst - thirstDepleteAmount, 0);
            thirstDepleteTimer = 0f;
        }
        
        fatigueDepleteTimer += Time.deltaTime; 
        if (fatigueDepleteTimer >= fatigueDepleteInterval) 
        {
            _currentFatigue = Mathf.Max(_currentFatigue - fatigueDepleteAmount, 0); 
            fatigueDepleteTimer = 0f; 
        }
        
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

        // Play "SHIVER" sound if cold is below 50% and hasn't played yet
        if (ColdPercent < 0.5f && !_hasShivered)
        {
            // Replace with your actual sound manager call
            SoundManager.PlaySound(SoundType.SHIVER);
            _hasShivered = true;
        }
        // Reset flag if cold goes above 50%
        else if (ColdPercent >= 0.5f)
        {
            _hasShivered = false;
        }
        #endregion
    }
    public void UpdateColdReplenishment(bool isNearFire, bool underShelter)
    {
        _inWarmZone = isNearFire || underShelter;
    }
    private void UpdateHealth()  // Update health based on critical stats 
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



    #region Item Stat Modifiers
    public void AddHealth(float amount)
    {
        // Clamp to max health
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
    }
    public void AddHunger(float amount)
    {
        _currentHunger = Mathf.Clamp(_currentHunger + amount, 0, _maxHunger);
        // Optionally update HungerPercent or UI here if needed
    }
    public void AddThirst(float amount)
    {
        _currentThirst = Mathf.Clamp(_currentThirst + amount, 0, _maxThirst);
        // Optionally update ThirstPercent or UI here if needed
    }

    public void AddFatigue(float amount)
    {
        _currentFatigue = Mathf.Clamp(_currentFatigue + amount, 0, _maxFatigue);
        // Optionally update FatiguePercent or UI here if needed
    }

    public void AddCold(float amount)
    {
        _currentCold = Mathf.Clamp(_currentCold + amount, 0, _maxCold);

        
        // Optionally update UI or trigger effects here
    }
    #endregion



    #region Health Management

    private void ApplyHealthDepletion()
    {
        #region Health Depletion
        float baseHealthDepletionRate = 1f;
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

    public void RegenerateHealth()
    {
        #region Health Regeneration
        if (_currentHealth < _maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + _healthRegenRate * Time.deltaTime, _maxHealth);
        }
        #endregion
    }
    #endregion

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

    

    private float DepleteStat(float currentValue, float depletionRate, float deltaTime, float minValue = 0)
    {
        currentValue -= depletionRate * deltaTime;
        return Mathf.Max(currentValue, minValue); // Ensure the value doesn't go below the minimum
        
    }

    
    private void HandlePlayerDeath()
    {
        if (!_playerHasDied)
        {
            Debug.Log("Player has died.");
            _playerHasDied = true;
        }
    }

   
}
