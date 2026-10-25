// Core/TimeManager.cs
using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Стартовое время")]
    [Tooltip("Стартовый час (0-23)")]
    [SerializeField] private int startHour = 8;

    [Tooltip("Стартовая минута (0-59)")]
    [SerializeField] private int startMinute = 0;

    [Tooltip("Стартовый день")]
    [SerializeField] private int startDay = 1;

    // Текущее время
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }
    public int CurrentDay { get; private set; }

    //Событие изменения времени. Передаёт (hour, minute, day)
    public event Action<int, int, int> OnTimeChanged;

    public event Action<int> OnDayChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentHour = Mathf.Clamp(startHour, 0, 23);
        CurrentMinute = Mathf.Clamp(startMinute, 0, 59);
        CurrentDay = startDay;
    }

    private void Start()
    {
        OnTimeChanged?.Invoke(CurrentHour, CurrentMinute, CurrentDay);
    }

    /// <summary>
    /// Продвигает время вперёд на указанное количество минут.
    /// </summary>
    public void Advance(int minutes)
    {
        if (minutes <= 0) return;

        CurrentMinute += minutes;

        // Переход через час
        while (CurrentMinute >= 60)
        {
            CurrentMinute -= 60;
            CurrentHour++;

            // Переход через полночь
            if (CurrentHour >= 24)
            {
                CurrentHour = 0;
                CurrentDay++;
                OnDayChanged?.Invoke(CurrentDay);
            }
        }

        OnTimeChanged?.Invoke(CurrentHour, CurrentMinute, CurrentDay);
    }

    /// <summary>
    /// Продвинуть на указанное количество часов.
    /// </summary>
    public void AdvanceHours(int hours) => Advance(hours * 60);

    /// <summary>
    /// Установить время напрямую (для сейвов, отладки).
    /// </summary>
    public void SetTime(int hour, int minute, int day)
    {
        CurrentHour = Mathf.Clamp(hour, 0, 23);
        CurrentMinute = Mathf.Clamp(minute, 0, 59);
        CurrentDay = day;
        OnTimeChanged?.Invoke(CurrentHour, CurrentMinute, CurrentDay);
    }

    /// <summary>
    /// Форматирует время в "HH:MM".
    /// </summary>
    public string GetFormattedTime() => $"{CurrentHour:00}:{CurrentMinute:00}";

    /// <summary>
    /// Возвращает время суток: "Утро", "День", "Вечер", "Ночь".
    /// </summary>
    public string GetTimeOfDay()
    {
        if (CurrentHour >= 5 && CurrentHour < 12) return "Утро";
        if (CurrentHour >= 12 && CurrentHour < 17) return "День";
        if (CurrentHour >= 17 && CurrentHour < 22) return "Вечер";
        return "Ночь";
    }
}