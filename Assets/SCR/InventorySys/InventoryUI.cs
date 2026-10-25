using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform slotsParent;
    public GameObject slotPrefab;

    private int rebuildsThisFrame = 0;
    private int lastFrame = -1;

    private void Start()
    {
        Inventory.Instance.OnInventoryChanged += Rebuild;
        Rebuild();
    }

    private void OnEnable()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnInventoryChanged -= Rebuild; // защита от дублей
            Inventory.Instance.OnInventoryChanged += Rebuild;
            Rebuild();
        }
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged -= Rebuild;
    }

    private void Rebuild()
    {
        if (Inventory.Instance == null) return;

        int itemCount = Inventory.Instance.items.Count;

        // Скрываем все слоты
        for (int i = 0; i < slotsParent.childCount; i++)
            slotsParent.GetChild(i).gameObject.SetActive(false);

        // Обновляем/создаем нужные
        for (int i = 0; i < itemCount; i++)
        {
            var item = Inventory.Instance.items[i];
            Transform slot;

            if (i < slotsParent.childCount)
            {
                slot = slotsParent.GetChild(i);
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot = Instantiate(slotPrefab, slotsParent).transform;
            }

            var icon = slot.GetComponentInChildren<Image>();
            if (icon != null) icon.sprite = item.icon;
            slot.GetComponent<InventorySlot>().Init(item);
        }

        Debug.Log($"[UI] Rebuild: items={itemCount}, active slots={itemCount}");
    }
}