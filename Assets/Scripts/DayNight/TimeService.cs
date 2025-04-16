using System;
using UnityEngine;
using UnityEngine.Rendering;

// This class manages the in-game time system, including time progression, day/night determination, 
// and sun angle calculations. It provides events for sunrise, sunset, and hourly changes.
public class TimeService 
{
    readonly TimeSettings settings;
    DateTime currentTime;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;

    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };

    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;

    /// <summary>
    /// Initializes the TimeService with the provided TimeSettings.
    /// Sets the initial time, sunrise/sunset times, and subscribes to day/night and hour change events.
    /// </summary>
    /// <param name="settings">The TimeSettings object containing configuration for time progression.</param>
    public TimeService(TimeSettings settings)
    {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);

        isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
    }

    /// <summary>
    /// Updates the in-game time based on the elapsed real-world time and the time multiplier.
    /// Triggers day/night and hour change events if applicable.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last frame, in seconds.</param>
    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
    }

    /// <summary>
    /// Calculates the sun's angle in the sky based on the current time.
    /// The angle ranges from 0° to 180° during the day and 180° to 360° during the night.
    /// </summary>
    /// <returns>The sun's angle in degrees.</returns>
    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();
        float startDegree = isDay ? 0 : 180;
        TimeSpan start = isDay ? sunriseTime : sunsetTime;
        TimeSpan end = isDay ? sunsetTime : sunriseTime;

        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
    }

    /// <summary>
    /// Gets the current in-game time.
    /// </summary>
    public DateTime CurrentTime => currentTime;

    /// <summary>
    /// Determines whether the current time is during the day (between sunrise and sunset).
    /// </summary>
    /// <returns>True if it is day, false if it is night.</returns>
    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    //Calculate the difference between two time spans to tell how close it is to sunrise or sunset
    /// <summary>
    /// Calculates the time difference between two TimeSpan values, accounting for wrapping around midnight.
    /// </summary>
    /// <param name="from">The starting time.</param>
    /// <param name="to">The ending time.</param>
    /// <returns>The time difference as a TimeSpan.</returns>
    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }

}
