using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Стартовое время")]
    [Tooltip("Стартовый час (0-23)")]
    [SerializeField] private int startHour = 6;

    [Tooltip("Стартовый день")]
    [SerializeField] private int startDay = 1;

    [Header("Настройки")]
    [Tooltip("Час, в который начинается новый день")]
    [SerializeField] private int dayStartHour = 6;

    //текущее время
    public int CurrentHour { get; private set; }
    public int CurrentDay { get; private set; }

    //события
    public event Action<int, int> OnTimeChanged;
    public event Action<int> OnDayChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentHour = Mathf.Clamp(startHour, 0, 23);
        CurrentDay = startDay;
    }

    private void Start()
    {
        OnTimeChanged?.Invoke(CurrentHour, CurrentDay);
    }

    public void Advance(int hours)
    {
        if (hours <= 0) return;

        for (int i = 0; i < hours; i++)
        {
            CurrentHour++;

            if (CurrentHour >= 24)
            {
                CurrentHour = 0;
                CurrentDay++;
                OnDayChanged?.Invoke(CurrentDay);
            }
            else if (CurrentHour == dayStartHour)
            {
                CurrentDay++;
                OnDayChanged?.Invoke(CurrentDay);
            }
        }

        OnTimeChanged?.Invoke(CurrentHour, CurrentDay);
    }

    public void SetTime(int hour, int day)
    {
        CurrentHour = Mathf.Clamp(hour, 0, 23);
        CurrentDay = day;
        OnTimeChanged?.Invoke(CurrentHour, CurrentDay);
    }

    public string GetFormattedTime() => $"{CurrentHour:00}:00";

    // время суток
    public string GetTimeOfDay()
    {
        if (CurrentHour >= 5 && CurrentHour < 12) return "Утро";
        if (CurrentHour >= 12 && CurrentHour < 17) return "День";
        if (CurrentHour >= 17 && CurrentHour < 22) return "Вечер";
        return "Ночь";
    }
}