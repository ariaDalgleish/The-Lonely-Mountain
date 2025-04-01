using UnityEngine;
using UnityEngine.Events;

public class SurvivalManager : MonoBehaviour
{
    [Header("Hunger")]
    [SerializeField] private float _maxHunger = 100f;
    [SerializeField] private float _hungerDepletionRate = 1f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;

    [Header("Thirst")]
    [SerializeField] private float _maxThirst = 100f;
    [SerializeField] private float _thirstDepletionRate = 2f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxThirst;

    [Header("Cold")]
    [SerializeField] private float _maxCold = 100f;
    [SerializeField] private float _coldDepletionRate = 5f;
    private float _currentCold;
    public float coldPercent => _currentCold / _maxCold;
    // Cold stats will be buffed with clothing and shelter
    // Cold will be affected by weather and time of day
    // Cold will affect fatigue and stamina

    [Header("Fatigue")]
    [SerializeField] private float _maxFatigue = 100f;
    [SerializeField] private float _fatigueDepletionRate = 5f;
    private float _currentFatigue;
    public float fatiguePercent => _currentFatigue / _maxFatigue;
    // Fatigue is replenished by sleep, and little will be replenished by eating/resting
    // Fatigue will affect max stamina

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 10f;
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 5f;
    private float _currentStamina;
    private float _currentStaminaDelayCounter;
    public float StaminaPercent => _currentStamina / _maxStamina;

    [Header("Player References")]
    [SerializeField] private PlayerControls _playerInput; 
    // Needs to reference Sprint input so change to InputManager instead?

    //public static UnityAction OnPlayerDied;

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
        _currentCold -= _coldDepletionRate * Time.deltaTime;
        _currentFatigue -= _fatigueDepletionRate * Time.deltaTime;

        if (_currentHunger <= 0 || _currentThirst <= 0 || _currentCold <=0 || _currentFatigue <=0)
        {
         //   OnPlayerDied?.Invoke();
            _currentHunger = 0;
            _currentThirst = 0;
            _currentCold = 0;
            _currentFatigue = 0;


        }

        if (_playerInput.sprint)
        {
            _currentStamina -= _staminaDepletionRate * Time.deltaTime;
            _currentStaminaDelayCounter = 0;
        }
        
        if (!_playerInput.sprint && _currentStamina < _maxStamina)
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







}
