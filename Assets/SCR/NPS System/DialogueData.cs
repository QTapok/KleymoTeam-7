using UnityEngine;

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

    [Header("Награда — предмет")]
    public ItemData rewardItem;
    [Min(0)] public int rewardItemCount = 1;

    [Header("Переход")]
    public int nextNodeIndex = -1;
}