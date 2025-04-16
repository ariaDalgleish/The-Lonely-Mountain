using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// This script manages the in-game time system, including updating the time of day, rotating the sun, 
// adjusting lighting settings, and blending the skybox for day-night transitions.

public class TimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;

    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;
    [SerializeField] Material skyboxMaterial;

    [SerializeField] TimeSettings timeSettings;


    ColorAdjustments colorAdjustments;

    // Service that handles time progression and sun angle calculations. 
    TimeService service;


    /// <summary>
    /// Initializes the TimeService and retrieves the ColorAdjustments component from the post-processing volume.
    /// </summary>
    private void Start()
    {
        service = new TimeService(timeSettings);
        volume.profile.TryGet(out colorAdjustments);
        
    }

    /// <summary>
    /// Updates the time of day, rotates the sun, adjusts lighting settings, and handles time multiplier controls.
    /// </summary>
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkyBlend();

        // Keycodes to control time for experimenting!
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            timeSettings.timeMultiplier *= 2;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            timeSettings.timeMultiplier /= 2;
        }
    }

    /// <summary>
    /// Updates the skybox blend based on the sun's position.
    /// </summary>
    void UpdateSkyBlend()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
        skyboxMaterial.SetFloat("_Blend", blend);
    }

    /// <summary>
    /// Adjusts the intensity of the sun and moon lights and the ambient light color based on the time of day.
    /// </summary>
    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
        moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(dotProduct));
        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(dotProduct));
    }

    /// <summary>
    /// Rotates the sun based on the calculated sun angle from the TimeService.
    /// </summary>

    void RotateSun() 
    { 
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);

    }

    /// <summary>
    /// Updates the TimeService and displays the current time in the UI.
    /// </summary>
    void UpdateTimeOfDay()
    {
        service.UpdateTime(Time.deltaTime);
        if (timeText != null)
        {
            timeText.text = service.CurrentTime.ToString("hh:mm");
        }
    }
}
