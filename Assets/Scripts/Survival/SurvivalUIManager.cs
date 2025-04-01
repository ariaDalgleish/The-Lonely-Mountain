using UnityEngine;
using UnityEngine.UI;

public class SurvivalUIManager : MonoBehaviour
{
    [SerializeField] private SurvivalManager _survivalManager;
    [SerializeField] private Image _hungerMeter, _thirstMeter, _staminaMeter, _warmthMeter, _restMeter;

    private void FixedUpdate()
    {
        _hungerMeter.fillAmount = _survivalManager.HungerPercent;
        _thirstMeter.fillAmount = _survivalManager.ThirstPercent;
        _staminaMeter.fillAmount = _survivalManager.StaminaPercent;
        _warmthMeter.fillAmount = _survivalManager.ColdPercent;
        _restMeter.fillAmount = _survivalManager.FatiguePercent;
    }
}
