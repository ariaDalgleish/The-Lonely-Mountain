using UnityEngine;
using UnityEngine.Events;

public class SurvivalManager : MonoBehaviour
{
    [Header("Hunger")]
    [SerializeField] private float _maxHunger = 100f;
    [SerializeField] private float _hungerDepletionRate = 2f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;

    [Header("Thirst")]
    [SerializeField] private float _maxThirst = 100f;
    [SerializeField] private float _thirstDepletionRate = 1f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxThirst;

    [Header("Cold")]
    [SerializeField] private float _maxCold = 100f;
    [SerializeField] private float _coldDepletionRate = 1f;
    [SerializeField] private float _coldReplenishRate = 2f; // New
    private float _currentCold;
    public float ColdPercent => _currentCold / _maxCold;

    [Header("Fatigue")]
    [SerializeField] private float _maxFatigue = 100f;
    [SerializeField] private float _fatigueDepletionRate = 1f;
    private float _currentFatigue;
    public float FatiguePercent => _currentFatigue / _maxFatigue;

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 5f;
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 5f;
    private float _currentStamina;
    private float _currentStaminaDelayCounter;
    public float StaminaPercent => _currentStamina / _maxStamina;

    private bool _isInColdReplenishZone = false; // New

    private void Start()
    {
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentStamina = _maxStamina;
        _currentCold = _maxCold;
        _currentFatigue = _maxFatigue;
    }

    private void Update()
    {
        _currentHunger -= _hungerDepletionRate * Time.deltaTime;
        _currentThirst -= _thirstDepletionRate * Time.deltaTime;

        if (_isInColdReplenishZone)
        {
            _currentCold += _coldReplenishRate * Time.deltaTime;
            if (_currentCold > _maxCold) _currentCold = _maxCold;
        }
        else
        {
            _currentCold -= _coldDepletionRate * Time.deltaTime;
        }

        _currentFatigue -= _fatigueDepletionRate * Time.deltaTime;

        if (_currentHunger <= 0 || _currentThirst <= 0)
        {
            //   OnPlayerDied?.Invoke();
            _currentHunger = 0;
            _currentThirst = 0;
        }

        if (InputManager.Instance.sprint)
        {
            _currentStamina -= _staminaDepletionRate * Time.deltaTime;
            if (_currentStamina <= 0) _currentStamina = 0;
            _currentStaminaDelayCounter = 0;
        }

        if (!InputManager.Instance.sprint && _currentStamina < _maxStamina)
        {
            if (_currentStaminaDelayCounter < _staminaRechargeDelay)
                _currentStaminaDelayCounter += Time.deltaTime;

            if (_currentStaminaDelayCounter >= _staminaRechargeDelay)
            {
                _currentStamina += _staminaRechargeRate * Time.deltaTime;
                if (_currentStamina > _maxStamina) _currentStamina = _maxStamina;
            }
        }
    }

    public void ReplenishHungerThirst(float hungerAmount, float thirstAmount)
    {
        _currentHunger += hungerAmount;
        _currentThirst += thirstAmount;
        if (_currentHunger > _maxHunger) _currentHunger = _maxHunger;
        if (_currentThirst > _maxThirst) _currentThirst = _maxThirst;
    }

    public void SetColdReplenishZone(bool isInZone) // New
    {
        _isInColdReplenishZone = isInZone;
    }

    public bool HasStamina()
    {
        return _currentStamina > 0;
    }
}