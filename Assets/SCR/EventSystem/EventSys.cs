using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSys : MonoBehaviour
{
    public static EventSys Instance { get; private set; }

    [Header("Все ивенты в игре")]
    [SerializeField] private GameEvent[] allEvents;


    private readonly HashSet<string> triggeredToday = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance == null) return;
        TimeManager.Instance.OnTimeChanged += OnHourChanged;
        TimeManager.Instance.OnDayChanged += OnNewDay;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance == null) return;
        TimeManager.Instance.OnTimeChanged -= OnHourChanged;
        TimeManager.Instance.OnDayChanged -= OnNewDay;
    }

    private void OnNewDay(int day)
    {
        triggeredToday.Clear();
    }

    private void OnHourChanged(int hour, int day)
    {
        foreach (var e in allEvents)
        {
            if (e == null) continue;
            if (e.oncePerDay && triggeredToday.Contains(e.id)) continue;

            if (e.trigger == EventTrigger.Random)
            {
                if (hour < e.minHour || hour > e.maxHour) continue;
                if (Random.value < e.chancePerHour)
                    Trigger(e);
            }
            else if (e.trigger == EventTrigger.Schedule)
            {
                if (hour == e.scheduleHour)
                    Trigger(e);
            }
        }
    }

    public bool TriggerById(string id)
    {
        foreach (var e in allEvents)
            if (e != null && e.id == id) { Trigger(e); return true; }
        return false;
    }

    public bool Trigger(GameEvent e)
    {
        if (e == null) return false;

        // Условия
        if (!string.IsNullOrEmpty(e.requiredLocation) && !IsPlayerAt(e.requiredLocation))
        {
            Debug.Log($"[Event] {e.displayName}: игрок не в {e.requiredLocation}");
            return false;
        }

        if (e.minWill > 0 && GetWill() < e.minWill)
        {
            Debug.Log($"[Event] {e.displayName}: воля ниже {e.minWill}");
            return false;
        }

        triggeredToday.Add(e.id);
        Debug.Log($"[Event] {e.displayName}: {e.description}");
        ApplyEffect(e);
        return true;
    }


    private void ApplyEffect(GameEvent e)
    {
        switch (e.effect)
        {
            case EventEffect.RemoveAllItems:
                Debug.Log("Обыск: всё из инвентаря изъято");
                break;

            case EventEffect.AddItem:
                Debug.Log($"Передача предмета: {e.itemId}");
                break;

            case EventEffect.ChangeWill:
                Debug.Log($"Воля изменена на {e.valueDelta:+0;-0}");
                break;

            case EventEffect.ChangeReputation:
                break;

            case EventEffect.ConfineToIsolation:
                Debug.Log("Игрок помещён в шизо");
                break;

            case EventEffect.Custom:
                HandleCustom(e);
                break;
        }
    }

    private void HandleCustom(GameEvent e)
    {
        switch (e.id)
        {
            case "roll_call":
                break;
            default:
                Debug.Log($"[Custom] {e.id} / {e.customParam}");
                break;
        }
    }

    private bool IsPlayerAt(string locationName) => true;
    private int GetWill() => 50;
}
