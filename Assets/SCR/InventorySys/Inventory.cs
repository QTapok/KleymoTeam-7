using System;
using System.Collections.Generic;
using UnityEngine;

// Инвентарь для хранения данных предметов
public class Inventory : MonoBehaviour
{
    // Статичная ссылка на инвентарь (Singleton)
    public static Inventory Instance;

    // Текущий выбранный предмет
    public ItemData SelectedItem { get; private set; }

    // Список предметов в инвентаре
    public List<ItemData> items = new();

    // Событие, которое вызывается при добавлении предмета
    public event Action<ItemData> OnItemAdded;
    // Событие, которое вызывается при смене выбранного предмета
    public event Action<ItemData> OnSelectionChanged;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        // Проверяем, чтобы в сцене был только один инвентарь
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Добавление предмета в инвентарь
    public void Add(ItemData item)
    {
        if (item == null) return;

        items.Add(item);

        // Уведомляем подписчиков о добавлении предмета
        OnItemAdded?.Invoke(item);

        OnInventoryChanged?.Invoke();
    }

    // Выбор предмета в инвентаре
    public void Select(ItemData item)
    {
        SelectedItem = item;
        // Уведомляем подписчиков об изменении выбранного предмета
        OnSelectionChanged?.Invoke(SelectedItem);
    }

    // Сброс выбранного предмета
    public void ClearSelection()
    {
        SelectedItem = null;
        // Уведомляем подписчиков об изменении выбранного предмета
        OnSelectionChanged?.Invoke(SelectedItem);
    }

    // Крафт и удаление предметов
    public bool TryCombine(ItemData a, ItemData b, out ItemData result)
    {
        result = null;

        Debug.Log($"[Inventory] TryCombine: {a?.title} + {b?.title}");

        if (a == null || b == null) return false;
        if (CraftingDatabase.Instance == null) return false;

        var recipe = CraftingDatabase.Instance.FindRecipe(a, b);
        if (recipe == null) return false;

        items.Remove(a);
        items.Remove(b);

        result = recipe.result;
        items.Add(result);   // ← добавляем результат

        if (SelectedItem == a || SelectedItem == b)
            ClearSelection();

        OnInventoryChanged?.Invoke();   // ← перерисовываем UI

        return true;
    }
}