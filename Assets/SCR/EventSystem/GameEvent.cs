using UnityEngine;
[CreateAssetMenu(fileName = "Event", menuName = "Prison/GameEvent")]
public class GameEvent : ScriptableObject
{
    [Header("ID")]
    public string id;
    public string displayName;
    [TextArea] public string description;

    [Header("”словие")]
    public EventTrigger trigger = EventTrigger.Random;

    [Tooltip("„ас в расписании")]
    public int scheduleHour = 6;

    [Tooltip("—лучайный час от и до")]
    public int minHour = 6;
    public int maxHour = 22;

    [Tooltip("Ўанс срабатывани€")]
    [Range(0f, 1f)] public float chancePerHour = 0.05f;

    [Tooltip("T/F один раз в день")]
    public bool oncePerDay = true;

    [Header("”слови€")]
    [Tooltip("“ребуема€ локаци€")]
    public string requiredLocation;

    [Tooltip("вол€ дл€ срабатывани€")] 
    public int minWill = 0;

    [Header("Ёффект")]
    public EventEffect effect = EventEffect.None;

    [Tooltip("id предмета")]
    public string itemId;

    [Tooltip("на сколько изменить")]
    public int valueDelta = 0;

    public string customParam;

}
public enum EventTrigger
{
    Random,
    Schedule,
    Manual
}
public enum EventEffect
{
    None,
    RemoveAllItems,
    AddItem,
    ChangeWill,
    ChangeHealth,
    ChangeReputation,
    ConfineToIsolation,
    Custom
}
