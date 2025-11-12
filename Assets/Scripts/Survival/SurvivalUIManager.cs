using UnityEngine;
using UnityEngine.UI;

public class SurvivalUIManager : MonoBehaviour
{
    [SerializeField] private SurvivalManager _survivalManager;
    [SerializeField] private Image _healthMeter, _staminaMeter, _warmthMeter, _restMeter, _thirstMeter, _hungerMeter;
    [SerializeField] private Image _healthIcon, _staminaIcon, _warmthIcon, _restIcon, _thirstIcon, _hungerIcon;
    [SerializeField] public GameObject Sprint;
    private CanvasGroup _sprintCanvasGroup;
    private bool sprintVisible = false;
    private Coroutine _fadeCoroutine;
    private InputManager _inputManager;
    private bool IsSprinting => _inputManager.GetSprintInput() && _survivalManager.HasStamina();

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _sprintCanvasGroup = Sprint.GetComponent<CanvasGroup>();
        if (_sprintCanvasGroup == null)
            _sprintCanvasGroup = Sprint.AddComponent<CanvasGroup>();
        _sprintCanvasGroup.alpha = 0f;
        Sprint.SetActive(false);
    }

    private void FixedUpdate()
    {
        _healthMeter.fillAmount = Mathf.Lerp(_healthMeter.fillAmount, _survivalManager.HealthPercent, Time.deltaTime * 5f);
        _healthMeter.color = _survivalManager.IsHealthCritical ? Color.red : Color.white;
        _healthIcon.color = _survivalManager.IsHealthCritical ? Color.red : Color.white;

        // Calculate the max fill percent relative to base max stamina
        float maxStaminaFillPercent = _survivalManager.MaxStamina / _survivalManager._baseMaxStamina;

        // Calculate the current fill percent relative to base max stamina
        float currentStaminaFillPercent = Mathf.Clamp01(_survivalManager._currentStamina / _survivalManager._baseMaxStamina);

        // Clamp the fill so it never exceeds the max stamina fill percent
        float staminaMeterFill = Mathf.Min(currentStaminaFillPercent, maxStaminaFillPercent);

        // Use this for your meter fill
        _staminaMeter.fillAmount = Mathf.Lerp(_staminaMeter.fillAmount, staminaMeterFill, Time.deltaTime * 5f);
        _staminaMeter.color = _survivalManager.IsStaminaCritical ? new Color(0.745283f, 0f, 0f, 1f) : Color.white;
        //_staminaIcon.color = _survivalManager.IsStaminaCritical ? new Color(0.745283f, 0f, 0f, 1f) : Color.white;

        _warmthMeter.fillAmount = Mathf.Lerp(_warmthMeter.fillAmount, _survivalManager.ColdPercent, Time.deltaTime * 5f);
        _warmthMeter.color = _survivalManager.IsColdCritical ? Color.red : Color.white;
        //_warmthIcon.color = _survivalManager.IsColdCritical ? Color.red : Color.white;

        _restMeter.fillAmount = Mathf.Lerp(_restMeter.fillAmount, _survivalManager.FatiguePercent, Time.deltaTime * 5f);
        _restMeter.color = _survivalManager.IsFatigueCritical ? Color.red : Color.white;
        _restIcon.color = _survivalManager.IsFatigueCritical ? Color.red : new Color(0.9715014f, 0.7122642f, 1f, 1f);
        
       
        _thirstMeter.fillAmount = Mathf.Lerp(_thirstMeter.fillAmount, _survivalManager.ThirstPercent, Time.deltaTime * 5f);
        _thirstMeter.color = _survivalManager.IsThirstCritical ? Color.red : Color.white;
        _thirstIcon.color = _survivalManager.IsThirstCritical ? Color.red : new Color(0.5618103f, 0.7935258f, 0.9528302f, 1f);

        _hungerMeter.fillAmount = Mathf.Lerp(_hungerMeter.fillAmount, _survivalManager.HungerPercent, Time.deltaTime * 5f);
        _hungerMeter.color = _survivalManager.IsHungerCritical ? Color.red : Color.white;
        _hungerIcon.color = _survivalManager.IsHungerCritical ? Color.red : new Color(1f, 0.7576767f, 0.3820755f, 1f); // RGBA for orange

        bool MaxStamina = _survivalManager._currentStamina >= _survivalManager.MaxStamina;
        //bool shouldShowSprint = IsSprinting && isStaminaNotFull;

        if (IsSprinting && !sprintVisible)
        {
            ShowSprintIcon();
        }
        else if (!IsSprinting && MaxStamina && sprintVisible)
        {
            HideSprintIcon();
        }
    }



    private void ShowSprintIcon()
    {
        Sprint.SetActive(true);
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_sprintCanvasGroup, _sprintCanvasGroup.alpha, 1f, 0.3f));
        sprintVisible = true;
    }

    private void HideSprintIcon()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutAndDisable(_sprintCanvasGroup, 0.3f));
        sprintVisible = false;
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    private System.Collections.IEnumerator FadeOutAndDisable(CanvasGroup cg, float duration)
    {
        yield return FadeCanvasGroup(cg, cg.alpha, 0f, duration);
        Sprint.SetActive(false);
    }

    public void SetMetersActive(bool active)
    {
        if (_healthMeter != null) _healthMeter.gameObject.SetActive(active);
        if (_staminaMeter != null) _staminaMeter.gameObject.SetActive(active);
        if (_warmthMeter != null) _warmthMeter.gameObject.SetActive(active);
        if (_restMeter != null) _restMeter.gameObject.SetActive(active);
        if (_thirstMeter != null) _thirstMeter.gameObject.SetActive(active);
        if (_hungerMeter != null) _hungerMeter.gameObject.SetActive(active);

        if (_healthIcon != null) _healthIcon.gameObject.SetActive(active);
        if (_staminaIcon != null) _staminaIcon.gameObject.SetActive(active);
        if (_warmthIcon != null) _warmthIcon.gameObject.SetActive(active);
        if (_restIcon != null) _restIcon.gameObject.SetActive(active);
        if (_thirstIcon != null) _thirstIcon.gameObject.SetActive(active);
        if (_hungerIcon != null) _hungerIcon.gameObject.SetActive(active);

        if (Sprint != null) Sprint.SetActive(active);
    }
}
