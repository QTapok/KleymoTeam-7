using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("Окно диалога")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;
    public GameObject Transit;
    public GameObject NPC;

    [Header("HUD — характеристики")]
    public TMP_Text reputationText;
    public TMP_Text willText;

    private DialogueData currentDialogue;
    private readonly List<Button> spawnedButtons = new();

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        // Подписываемся на изменение статов — UI будет обновляться сам
        PlayerStats.OnStatsChanged += UpdateStatsUI;
        UpdateStatsUI();
    }

    void OnDestroy()
    {
        // Отписываемся, чтобы не было утечек
        PlayerStats.OnStatsChanged -= UpdateStatsUI;
    }

    // ==== Публичное API ====

    public void Open(DialogueData data)
    {
        currentDialogue = data;
        panel.SetActive(true);
        ShowNode(0);
        Transit.SetActive(false);
        NPC.SetActive(false);
    }

    public void Close()
    {
        Transit.SetActive(true);
        NPC.SetActive(true);
        panel.SetActive(false);
        currentDialogue = null;
    }

    // ==== Показ узла ====

    private void ShowNode(int index)
    {
        // Удаляем старые кнопки
        foreach (var b in spawnedButtons) Destroy(b.gameObject);
        spawnedButtons.Clear();

        // Защита от некорректного индекса
        if (currentDialogue == null || index < 0 || index >= currentDialogue.nodes.Length)
        {
            Close();
            return;
        }

        var node = currentDialogue.nodes[index];
        dialogueText.text = node.npcText;

        foreach (var choice in node.choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);

            // Текст кнопки — плюс подсказка о последствиях (опционально)
            string hint = BuildHint(choice);
            btn.GetComponentInChildren<TMP_Text>().text = choice.choiceText + hint;

            var c = choice; // локальная копия для замыкания
            btn.onClick.AddListener(() => OnChoiceSelected(c));
            spawnedButtons.Add(btn);
        }
    }

    // Подсказка вида " (Репутация +1, Воля -1)" — можно отключить, убрав вызов
    private string BuildHint(DialogueChoice choice)
    {
        if (choice.reputationDelta == 0 && choice.willDelta == 0)
            return "";

        var parts = new List<string>();
        if (choice.reputationDelta != 0)
            parts.Add($"Репутация {choice.reputationDelta:+#;-#;0}");
        if (choice.willDelta != 0)
            parts.Add($"Воля {choice.willDelta:+#;-#;0}");

        return "  <size=70%>(" + string.Join(", ", parts) + ")</size>";
    }

    // ==== Обработка выбора ====

    private void OnChoiceSelected(DialogueChoice choice)
    {
        // Применяем оба стата одной командой — UI обновится через событие
        PlayerStats.Apply(choice.reputationDelta, choice.willDelta);

        if (choice.nextNodeIndex < 0)
            Close();
        else
            ShowNode(choice.nextNodeIndex);
    }

    // ==== Обновление HUD ====

    private void UpdateStatsUI()
    {
        if (reputationText != null)
            reputationText.text = $"Репутация: {PlayerStats.Reputation}";

        if (willText != null)
            willText.text = $"Воля: {PlayerStats.Will}";
    }
}
