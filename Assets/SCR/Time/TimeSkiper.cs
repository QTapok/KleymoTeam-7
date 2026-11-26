using UnityEngine;

[CreateAssetMenu(menuName = "Prison/Action", fileName = "Action_")]
public class TimeSkiper : ScriptableObject
{
    public string id;
    public string displayName;
    [Tooltip("—колько часов занимает действие")]
    public int hoursCost = 1;

    [Tooltip("ћожно ли выполнить в этот час")]
    public bool timeRestricted = false;
    public int minHour = 0;
    public int maxHour = 23;

    [Tooltip("дл€ проверки локации")]
    public string requiredLocation;
}
