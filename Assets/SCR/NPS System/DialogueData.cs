using UnityEngine;
public enum StatRequirementMode
{
    Any = 0,
    GreaterOrEqual,
    LessOrEqual,
    Greater,
    Less,
    Equal,
    NotEqual
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Quest/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueNode[] nodes;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea] public string npcText;
    public DialogueChoice[] choices;

    [Header("Если все варианты скрыты — куда перейти (-1 = закрыть)")]
    public int fallbackNodeIndex = -1;
}

[System.Serializable]
public class DialogueChoice
{
    [Header("Текст")]
    public string choiceText;
    [TextArea] public string responseText;

    [Header("Требования — предмет")]
    public ItemData requiredItem;
    [Min(1)] public int requiredItemCount = 1;
    public bool consumeRequiredItem = true;

    [Header("Требования — репутация")]
    [Tooltip("Any — не проверять. Greater — строго больше, GreaterOrEqual — больше либо равно")]
    public StatRequirementMode reputationRequirement = StatRequirementMode.Any;
    public int reputationThreshold = 0;

    [Header("Требования — воля")]
    [Tooltip("Any — не проверять. Less — строго меньше, LessOrEqual — меньше либо равно")]
    public StatRequirementMode willRequirement = StatRequirementMode.Any;
    public int willThreshold = 0;

    [Header("Требования — флаги")]
    public string[] requiredFlags = new string[0];
    public string[] forbiddenFlags = new string[0];

    [Header("Показывать только один раз")]
    public bool onceOnly = false;

    [Header("Флаги при выборе")]
    public string[] setFlags = new string[0];
    public string[] clearFlags = new string[0];

    [Header("Последствия — статы")]
    public int reputationDelta;
    public int willDelta;
    public int healthelta;

    [Header("Награда — предмет")]
    public ItemData rewardItem;
    [Min(0)] public int rewardItemCount = 1;

    [Header("Награда — рецепты")]
    public CraftRecipe[] unlockRecipes;

    [Header("Переход")]
    public int nextNodeIndex = -1;

    public static bool MeetsRequirement(StatRequirementMode mode, int value, int threshold)
    {
        switch (mode)
        {
            case StatRequirementMode.Any: return true;
            case StatRequirementMode.GreaterOrEqual: return value >= threshold;
            case StatRequirementMode.LessOrEqual: return value <= threshold;
            case StatRequirementMode.Greater: return value > threshold;
            case StatRequirementMode.Less: return value < threshold;
            case StatRequirementMode.Equal: return value == threshold;
            case StatRequirementMode.NotEqual: return value != threshold;
            default: return true;
        }
    }

    public static string FormatRequirement(string statName, StatRequirementMode mode, int threshold)
    {
        switch (mode)
        {
            case StatRequirementMode.GreaterOrEqual: return statName + " ≥ " + threshold;
            case StatRequirementMode.LessOrEqual: return statName + " ≤ " + threshold;
            case StatRequirementMode.Greater: return statName + " > " + threshold;
            case StatRequirementMode.Less: return statName + " < " + threshold;
            case StatRequirementMode.Equal: return statName + " = " + threshold;
            case StatRequirementMode.NotEqual: return statName + " ≠ " + threshold;
            default: return statName;
        }
    }
}