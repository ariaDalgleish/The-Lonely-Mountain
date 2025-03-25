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
    [SerializeField] private float _thirstDepletionRate = 1f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxThirst;

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _staminaDepletionRate = 1f;
    [SerializeField] private float _staminaRechargeRate = 2f;
    [SerializeField] private float _staminaRechargeDelay = 1f;
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
    }

    private void Update()
    {
        _currentHunger -= _hungerDepletionRate * Time.deltaTime;
        _currentThirst -= _thirstDepletionRate * Time.deltaTime;

        if (_currentHunger <= 0 || _currentThirst <= 0)
        {
         //   OnPlayerDied?.Invoke();
            _currentHunger = 0;
            _currentThirst = 0;
        }
    }
}
