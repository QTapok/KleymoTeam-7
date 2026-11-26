using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public ItemData SelectedItem { get; private set; }

    public List<ItemData> items = new();

    public event Action<ItemData> OnItemAdded;
    public event Action<ItemData> OnSelectionChanged;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Add(ItemData item)
    {
        if (item == null) return;

        items.Add(item);

        OnItemAdded?.Invoke(item);

        OnInventoryChanged?.Invoke();
    }

    public void Select(ItemData item)
    {
        SelectedItem = item;
        OnSelectionChanged?.Invoke(SelectedItem);
    }

    public void ClearSelection()
    {
        SelectedItem = null;
        OnSelectionChanged?.Invoke(SelectedItem);
    }

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
        items.Add(result);

        if (SelectedItem == a || SelectedItem == b)
            ClearSelection();

        OnInventoryChanged?.Invoke();

        return true;
    }

    public int Count(ItemData item)
    {
        if (item == null) return 0;
        int c = 0;
        foreach (var i in items) if (i == item) c++;
        return c;
    }

    public bool Has(ItemData item, int amount = 1)
        => amount <= 0 ? true : Count(item) >= amount;

    public bool Remove(ItemData item)
    {
        if (item == null) return false;
        if (!items.Remove(item)) return false;

        if (SelectedItem == item) ClearSelection();

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int Remove(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        int removed = 0;
        while (removed < amount && items.Remove(item)) removed++;

        if (removed > 0)
        {
            if (SelectedItem == item && !items.Contains(item))
                ClearSelection();

            OnInventoryChanged?.Invoke();
        }
        return removed;
    }

    public void ClearAll()
    {
        if (items.Count == 0) return;
        items.Clear();
        ClearSelection();
        OnInventoryChanged?.Invoke();
    }
}