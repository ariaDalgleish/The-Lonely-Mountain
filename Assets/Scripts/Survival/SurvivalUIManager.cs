using UnityEngine;
using UnityEngine.UI;

public class SurvivalUIManager : MonoBehaviour
{


    [SerializeField] private SurvivalManager _survivalManager;
    [SerializeField] private Image _hungerMeter, _thirstMeter, _staminaMeter, _warmthMeter, _restMeter, _healthMeter;

    private void FixedUpdate()
    {
        _hungerMeter.fillAmount = Mathf.Lerp(_hungerMeter.fillAmount, _survivalManager.HungerPercent, Time.deltaTime * 5f);
        _thirstMeter.fillAmount = Mathf.Lerp(_thirstMeter.fillAmount, _survivalManager.ThirstPercent, Time.deltaTime * 5f);
        _staminaMeter.fillAmount = Mathf.Lerp(_staminaMeter.fillAmount, _survivalManager.StaminaPercent, Time.deltaTime * 5f);
        _warmthMeter.fillAmount = Mathf.Lerp(_warmthMeter.fillAmount, _survivalManager.ColdPercent, Time.deltaTime * 5f);
        _restMeter.fillAmount = Mathf.Lerp(_restMeter.fillAmount, _survivalManager.FatiguePercent, Time.deltaTime * 5f);
        _healthMeter.fillAmount = Mathf.Lerp(_healthMeter.fillAmount, _survivalManager.HealthPercent, Time.deltaTime * 5f);
    }
}
