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
}

[System.Serializable]
public class DialogueChoice
{
    [Header("Текст")]
    public string choiceText;

    [TextArea] public string responseText;

    [Header("Последствия")]
    public int reputationDelta;   // + / - к репутации
    public int willDelta;         // + / - к воле

    [Header("Переход")]
    public int nextNodeIndex = -1;
}
