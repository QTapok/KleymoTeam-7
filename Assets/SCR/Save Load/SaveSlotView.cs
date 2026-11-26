using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotView : MonoBehaviour
{
    [Header("Текст")]
    [SerializeField] private TMP_Text dayHourText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text emptyText;

    [Header("Кнопки (ОБЯЗАТЕЛЬНО ЗАПОЛНИТЬ В ИНСПЕКТОРЕ!)")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    private int _slot = -1;
    private System.Action<int> _onNewGame;
    private System.Action<int> _onLoad;
    private System.Action<int> _onDelete;

    private void Awake()
    {
        if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGameClicked);
        if (loadButton != null) loadButton.onClick.AddListener(OnLoadClicked);
        if (deleteButton != null) deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void Bind(int slot, SaveData data, System.Action<int> onNewGame, System.Action<int> onLoad, System.Action<int> onDelete)
    {
        _slot = slot;
        _onNewGame = onNewGame;
        _onLoad = onLoad;
        _onDelete = onDelete;

        bool exists = data != null;

        if (newGameButton != null) newGameButton.gameObject.SetActive(!exists);
        if (loadButton != null) loadButton.interactable = exists;
        if (deleteButton != null) deleteButton.interactable = exists;
        if (emptyText != null) emptyText.gameObject.SetActive(!exists);

        if (!exists)
        {
            if (dayHourText != null) dayHourText.text = $"Слот {slot + 1} (Пусто)";
            if (statsText != null) statsText.text = "Нажмите 'Новая игра', чтобы начать";
            return;
        }

        if (dayHourText != null)
            dayHourText.text = $"Слот {slot + 1} · День {data.day} · {data.hour:00}:00";

        if (statsText != null)
            statsText.text =
                $"Репутация: {data.reputation}   Воля: {data.will}\n" +
                $"Здоровье: {data.health}   Голод: {data.hungry}   Усталость: {data.fatigue}";
    }

    private void OnNewGameClicked()
    {
        Debug.Log($"[SaveSlot] Клик по Новой игре в слоте {_slot}");
        _onNewGame?.Invoke(_slot);
    }

    private void OnLoadClicked()
    {
        Debug.Log($"[SaveSlot] Клик по Загрузке в слоте {_slot}");
        _onLoad?.Invoke(_slot);
    }

    private void OnDeleteClicked()
    {
        Debug.Log($"[SaveSlot] Клик по Удалению в слоте {_slot}");
        _onDelete?.Invoke(_slot);
    }
}