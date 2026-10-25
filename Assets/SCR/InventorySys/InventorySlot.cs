// InventorySlot.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private ItemData item;
    private static GameObject dragGhost;
    private static ItemData draggedItem;

    private void Start()
    {
        DebugLogger.Log($"[Slot] Start, item = {(item != null ? item.title : "NULL")}");
    }
    public ItemData Item => item;

    public void Init(ItemData data) => item = data;

    // ---------- Клики ----------
    public void OnPointerClick(PointerEventData eventData)
    {
        DebugLogger.Log($"[Slot] OnPointerClick: button={eventData.button}, item={item?.title}");
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemDescriptionUI.Instance.Show(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.Instance.SelectedItem == item)
                Inventory.Instance.ClearSelection();
            else
                Inventory.Instance.Select(item);
        }
    }

    // ---------- Drag ----------
    public void OnBeginDrag(PointerEventData eventData)
    {
        DebugLogger.Log($"[Slot] OnBeginDrag: {item.title}");
        if (item == null) return;

        draggedItem = item;

        

        dragGhost = new GameObject("DragGhost", typeof(Image));
        var img = dragGhost.GetComponent<Image>();
        img.sprite = item.icon;
        img.raycastTarget = false;

        Canvas canvas = GetComponentInParent<Canvas>();
        dragGhost.transform.SetParent(canvas.transform, false);
        dragGhost.transform.SetAsLastSibling();

        var srcIcon = GetComponentInChildren<Image>();
        if (srcIcon != null)
            dragGhost.GetComponent<RectTransform>().sizeDelta =
                srcIcon.rectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost == null) return;

        var canvas = dragGhost.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint))
        {
            dragGhost.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DebugLogger.Log($"[Slot] OnEndDrag: draggedItem = {(draggedItem != null ? draggedItem.title : "NULL")}");

        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }

        draggedItem = null;
    }

    // ---------- Drop ----------
    public void OnDrop(PointerEventData eventData)
    {
        DebugLogger.Log($"[Slot] OnDrop вызван! draggedItem = {(draggedItem != null ? draggedItem.title : "NULL")}, item = {(item != null ? item.title : "NULL")}");

        // Сразу уничтожаем ghost — drag завершен
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }

        if (draggedItem == null || item == null)
        {
            draggedItem = null;
            return;
        }

        if (draggedItem == item)
        {
            draggedItem = null;
            return;
        }

        if (Inventory.Instance.TryCombine(draggedItem, item, out ItemData result))
        {
            DebugLogger.Log($"Скрафчено: {result.title}");
        }
        else
        {
            DebugLogger.Log("Рецепт не найден");
        }

        draggedItem = null;
    }
}