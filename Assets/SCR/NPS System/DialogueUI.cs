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
    private readonly List<Button> spawnedButtons = new List<Button>();

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        PlayerStats.OnStatsChanged += UpdateStatsUI;
        UpdateStatsUI();
    }

    void OnDestroy()
    {
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

    private void ShowNode(int index)
    {
        foreach (var b in spawnedButtons) Destroy(b.gameObject);
        spawnedButtons.Clear();

        if (currentDialogue == null || index < 0 || index >= currentDialogue.nodes.Length)
        {
            Close();
            return;
        }

        var node = currentDialogue.nodes[index];
        dialogueText.text = node.npcText;

        int shown = 0;

        for (int i = 0; i < node.choices.Length; i++)
        {
            var choice = node.choices[i];
            if (choice == null) continue;

            if (!IsChoiceAvailable(choice, index, i)) continue;

            bool hasItem = HasRequiredItem(choice, out string missing);

            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            btn.GetComponentInChildren<TMP_Text>().text =
                choice.choiceText + BuildHint(choice, hasItem, missing);

            if (!hasItem)
            {
                btn.interactable = false;
                var c = btn.colors;
                c.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.7f);
                btn.colors = c;
            }

            int capturedNode = index;
            int capturedChoice = i;
            var capturedRef = choice;
            btn.onClick.AddListener(() =>
                OnChoiceSelected(capturedRef, capturedNode, capturedChoice));

            spawnedButtons.Add(btn);
            shown++;
        }

        if (shown == 0)
        {
            if (node.fallbackNodeIndex >= 0)
                ShowNode(node.fallbackNodeIndex);
            else
                Close();
        }
    }

    private bool IsChoiceAvailable(DialogueChoice choice, int nodeIdx, int choiceIdx)
    {
        if (choice.onceOnly && DialogueFlags.IsChoiceUsed(KeyFor(nodeIdx, choiceIdx)))
            return false;

        if (choice.requiredFlags != null)
            foreach (var f in choice.requiredFlags)
                if (!DialogueFlags.Has(f)) return false;

        if (choice.forbiddenFlags != null)
            foreach (var f in choice.forbiddenFlags)
                if (DialogueFlags.Has(f)) return false;

        return true;
    }

    private string KeyFor(int nodeIdx, int choiceIdx)
    {
        string dlgName = currentDialogue != null ? currentDialogue.name : "null";
        return dlgName + "|n" + nodeIdx + "|c" + choiceIdx;
    }

    private bool HasRequiredItem(DialogueChoice choice, out string missing)
    {
        missing = null;
        if (choice.requiredItem == null) return true;

        if (Inventory.Instance == null)
        {
            missing = choice.requiredItem.title;
            return false;
        }

        if (!Inventory.Instance.Has(choice.requiredItem, choice.requiredItemCount))
        {
            missing = choice.requiredItem.title;
            if (choice.requiredItemCount > 1) missing += " ×" + choice.requiredItemCount;
            return false;
        }
        return true;
    }

    private string BuildHint(DialogueChoice choice, bool hasItem, string missing)
    {
        var parts = new List<string>();

        if (choice.requiredItem != null)
        {
            string req = hasItem
                ? "Требуется: " + choice.requiredItem.title
                : "Требуется: " + (missing ?? choice.requiredItem.title);
            if (hasItem && choice.requiredItemCount > 1)
                req += " ×" + choice.requiredItemCount;
            parts.Add(hasItem ? req : "<color=#FF6B6B>" + req + "</color>");
        }

        if (choice.reputationDelta != 0)
            parts.Add("Репутация " + choice.reputationDelta.ToString("+#;-#;0"));
        if (choice.willDelta != 0)
            parts.Add("Воля " + choice.willDelta.ToString("+#;-#;0"));

        if (choice.rewardItem != null && choice.rewardItemCount > 0)
        {
            string rew = "Награда: " + choice.rewardItem.title;
            if (choice.rewardItemCount > 1) rew += " ×" + choice.rewardItemCount;
            parts.Add(rew);
        }

        if (parts.Count == 0) return "";
        return "  <size=70%>(" + string.Join(", ", parts) + ")</size>";
    }

    private void OnChoiceSelected(DialogueChoice choice, int nodeIdx, int choiceIdx)
    {
        if (!IsChoiceAvailable(choice, nodeIdx, choiceIdx)) return;
        if (!HasRequiredItem(choice, out _)) return;

        if (choice.requiredItem != null && choice.consumeRequiredItem &&
            Inventory.Instance != null)
        {
            int removed = Inventory.Instance.Remove(choice.requiredItem, choice.requiredItemCount);
            if (removed < choice.requiredItemCount)
            {
                Debug.LogWarning("[Dialogue] Не удалось забрать " +
                    choice.requiredItem.title + " ×" + choice.requiredItemCount);
                return;
            }
        }

        PlayerStats.Apply(choice.reputationDelta, choice.willDelta);

        if (choice.rewardItem != null && choice.rewardItemCount > 0 &&
            Inventory.Instance != null)
        {
            for (int i = 0; i < choice.rewardItemCount; i++)
                Inventory.Instance.Add(choice.rewardItem);
        }

        if (choice.onceOnly)
            DialogueFlags.MarkChoiceUsed(KeyFor(nodeIdx, choiceIdx));

        if (choice.setFlags != null)
            foreach (var f in choice.setFlags) DialogueFlags.Set(f);
        if (choice.clearFlags != null)
            foreach (var f in choice.clearFlags) DialogueFlags.Clear(f);

        if (choice.nextNodeIndex < 0) Close();
        else ShowNode(choice.nextNodeIndex);
    }

    private void UpdateStatsUI()
    {
        if (reputationText != null)
            reputationText.text = "Репутация: " + PlayerStats.Reputation;
        if (willText != null)
            willText.text = "Воля: " + PlayerStats.Will;
    }
}