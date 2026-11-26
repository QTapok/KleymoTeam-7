using System;
using System.Collections.Generic;
using UnityEngine;

public class DailySchedule : MonoBehaviour
{
    public enum PenaltyType
    {
        None,
        Warning,
        RemoveAllItems,
        ConfineToIsolation,
        ChangeWill,
        ChangeReputation,
        ForceSleep
    }

    [Serializable]
    public class ScheduleItem
    {
        public string id;
        public string displayName;
        public int hour;

        [Tooltip("id локации, где обязан быть игрок")]
        public string requiredLocationId;

        [Tooltip("Что делать при пропуске")]
        public PenaltyType penalty = PenaltyType.Warning;

        [Tooltip("на сколько изменить волю")]
        public int willDelta = -10;

        [Tooltip("на сколько изменить репутацию")]
        public int reputationDelta = -5;

        [Tooltip("Сообщение игроку при пропуске")]
        [TextArea] public string penaltyMessage;
    }

    [Header("Расписание на день")]
    [SerializeField] private ScheduleItem[] items;

    [Header("Мягкая проверка")]
    [SerializeField] private bool softCheck = true;

    [Header("Локация шизо")]
    [Tooltip("id карцера")]
    [SerializeField] private string isolationLocationId = "isolation";

    private readonly HashSet<string> visitedToday = new HashSet<string>();

    private void Start()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("TimeManager.Instance == null");
            return;
        }

        TimeManager.Instance.OnTimeChanged += OnHourChanged;
        TimeManager.Instance.OnDayChanged += OnNewDay;

        if (softCheck && LocationManager.Instance != null)
            LocationManager.Instance.OnLocationChanged += OnLocationChanged;

        OnHourChanged(TimeManager.Instance.CurrentHour, TimeManager.Instance.CurrentDay);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged -= OnHourChanged;
            TimeManager.Instance.OnDayChanged -= OnNewDay;
        }
        if (softCheck && LocationManager.Instance != null)
            LocationManager.Instance.OnLocationChanged -= OnLocationChanged;
    }

    private void OnNewDay(int day)
    {
        visitedToday.Clear();
        Debug.Log($"День {day}:");
    }

    private void OnLocationChanged(string oldId, string newId)
    {
        if (!softCheck) return;
        if (TimeManager.Instance == null) return;

        int hour = TimeManager.Instance.CurrentHour;
        foreach (var item in items)
        {
            if (item == null) continue;
            if (item.requiredLocationId != newId) continue;
            if (item.hour != hour) continue;
            visitedToday.Add(item.id);
            Debug.Log($"{item.displayName}: отмечено при входе в '{newId}'");
        }
    }

    private void OnHourChanged(int hour, int day)
    {
        if (SaveManager.Instance != null && SaveManager.Instance.IsLoading) return;

        if (SaveManager.Instance != null && SaveManager.Instance.IsLoading) return;
        foreach (var item in items)
        {
            if (item == null) continue;
            if (item.hour != hour) continue;

            bool ok = softCheck
                ? visitedToday.Contains(item.id)
                : IsPlayerAt(item.requiredLocationId);

            if (ok)
            {
                Debug.Log($"[Schedule] {item.displayName}: засчитано");
                visitedToday.Add(item.id);
            }
            else
            {
                string where = LocationManager.Instance?.Current?.id ?? "?";
                Debug.LogWarning($"[Schedule] {item.displayName}: ПРОПУСК" +
                                 $"(игрок в '{where}', нужен '{item.requiredLocationId}')");
                ApplyPenalty(item);
            }
        }
    }

    public void MarkVisited(string scheduleId)
    {
        visitedToday.Add(scheduleId);
        Debug.Log($"Отмечено вручную: {scheduleId}");
    }

    private bool IsPlayerAt(string requiredLocationId)
    {
        if (string.IsNullOrEmpty(requiredLocationId)) return true;
        if (LocationManager.Instance == null || LocationManager.Instance.Current == null) return true;
        return LocationManager.Instance.Current.id == requiredLocationId;
    }

    private void ApplyPenalty(ScheduleItem item)
    {
        if (!string.IsNullOrEmpty(item.penaltyMessage))
            Debug.Log($"{item.penaltyMessage}");

        switch (item.penalty)
        {
            case PenaltyType.None:
                break;

            case PenaltyType.Warning:
                break;

            case PenaltyType.RemoveAllItems:
                if (Inventory.Instance != null)
                    Inventory.Instance.ClearAll();
                GameState.Instance.SetFlag($"penalty_search_day{TimeManager.Instance.CurrentDay}");
                Debug.Log("Инвентарь изъят");

                if (!string.IsNullOrEmpty(isolationLocationId) && LocationManager.Instance != null)
                {
                    LocationManager.Instance.GoTo(isolationLocationId);
                    Debug.Log($"Игрок перемещен в {isolationLocationId}");
                }
                break;

            case PenaltyType.ConfineToIsolation:
                GameState.Instance.SetFlag($"in_isolation_day{TimeManager.Instance.CurrentDay}");
                if (!string.IsNullOrEmpty(isolationLocationId) && LocationManager.Instance != null)
                    LocationManager.Instance.GoTo(isolationLocationId);
                Debug.Log("Игрок отправлен в шизо");
                break;

            case PenaltyType.ChangeWill:
                PlayerStats.AddWill(item.willDelta);
                break;

            case PenaltyType.ChangeReputation:
                PlayerStats.AddReputation(item.reputationDelta);
                break;
            case PenaltyType.ForceSleep:
                if (SleepManager.Instance != null && !SleepManager.Instance.IsSleeping)
                {
                    Debug.Log("Игрок не в постели");
                    SleepManager.Instance.KnockOut();
                }
                break;
        }
    }
}