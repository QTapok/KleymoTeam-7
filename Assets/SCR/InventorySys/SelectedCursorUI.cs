using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class SelectedCursorUI : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private Vector2 offset = new Vector2(1f, -1f);

    [Header("Hover Detection")]
    [Tooltip("Слои объектов, на которые реагируем")]
    [SerializeField] private LayerMask hoverLayerMask = ~0;
    [Tooltip("Спрайт, если у объекта есть коллайдер, но нет HoverIcon/icon")]
    [SerializeField] private Sprite fallbackHoverSprite;

    [Header("Default")]
    [Tooltip("Твой нарисованный курсор: показывается, когда нет предмета и нет наведения")]
    [SerializeField] private Sprite defaultCursorSprite;

    [Header("System Cursor")]
    [Tooltip("Скрывать системный курсор, пока этот компонент активен")]
    [SerializeField] private bool hideSystemCursor = true;

    private Image image;
    private RectTransform rect;
    private Canvas canvas;

    private Sprite heldSprite;   // иконка предмета в руке
    private Sprite hoverSprite;  // иконка объекта под курсором
    private Sprite appliedSprite;

    private bool subscribed;
    private bool cursorHidden;

    // ------------------------------------------------------------
    // Lifecycle
    // ------------------------------------------------------------
    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        image.raycastTarget = false;

        // Сразу показываем дефолтный курсор (если задан), иначе Image скрыт
        ApplySprite(defaultCursorSprite);
    }

    private void OnEnable()
    {
        TrySubscribe();
        if (hideSystemCursor) SetSystemCursorVisible(false);
    }

    private void Start()
    {
        TrySubscribe();
        RefreshHeldItem();
    }

    private void OnDisable()
    {
        Unsubscribe();
        if (cursorHidden) SetSystemCursorVisible(true);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && cursorHidden)
            SetSystemCursorVisible(true);
        else if (hasFocus && hideSystemCursor && isActiveAndEnabled)
            SetSystemCursorVisible(false);
    }

    private void OnApplicationQuit()
    {
        if (cursorHidden) SetSystemCursorVisible(true);
    }

    private void SetSystemCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        cursorHidden = !visible;
    }

    // ------------------------------------------------------------
    // Inventory
    // ------------------------------------------------------------
    private void TrySubscribe()
    {
        if (subscribed) return;
        if (Inventory.Instance == null) return;

        Inventory.Instance.OnSelectionChanged += HandleSelectionChanged;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        if (Inventory.Instance != null)
            Inventory.Instance.OnSelectionChanged -= HandleSelectionChanged;
        subscribed = false;
    }

    private void HandleSelectionChanged(ItemData selected)
    {
        heldSprite = (selected != null) ? selected.icon : null;
        Reevaluate();
    }

    private void RefreshHeldItem()
    {
        if (heldSprite == null && Inventory.Instance != null)
        {
            var current = Inventory.Instance.SelectedItem; // если есть
            if (current != null) heldSprite = current.icon;
        }
        Reevaluate();
    }

    // ------------------------------------------------------------
    // Update
    // ------------------------------------------------------------
    private void Update()
    {
        if (canvas == null) return;

        Vector2 screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Vector2.zero;

        PositionAtMouse(screenPos);
        UpdateHover(screenPos);
        Reevaluate();
    }

    private void PositionAtMouse(Vector2 screenPos)
    {
        RectTransform parentRect = rect.parent as RectTransform;
        if (parentRect == null) return;

        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenPos, uiCam, out Vector2 localPoint))
        {
            rect.anchoredPosition = localPoint + offset;
        }
    }

    private void UpdateHover(Vector2 screenPos)
    {
        // 1) Над UI — мир игнорируем
        if (IsPointerOverUI(screenPos))
        {
            hoverSprite = null;
            return;
        }

        // 2) Камера
        Camera worldCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? Camera.main
            : canvas.worldCamera;

        if (worldCam == null)
        {
            hoverSprite = null;
            return;
        }

        // 3) Рейкаст
        Vector2 worldPos = worldCam.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, hoverLayerMask);

        if (hit == null)
        {
            hoverSprite = null;
            return;
        }

        // 4) Маркер
        HoverIcon marker = hit.GetComponentInParent<HoverIcon>();
        hoverSprite = (marker != null && marker.icon != null)
            ? marker.icon
            : fallbackHoverSprite; // может быть null
    }

    private bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        var ped = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        foreach (var r in results)
        {
            if (r.gameObject == gameObject) continue;
            return true;
        }
        return false;
    }

    // ------------------------------------------------------------
    // Применение спрайта: hover > held > default
    // ------------------------------------------------------------
    private void Reevaluate()
    {
        Sprite wanted = hoverSprite != null
            ? hoverSprite
            : (heldSprite != null ? heldSprite : defaultCursorSprite);

        if (wanted == appliedSprite) return;
        ApplySprite(wanted);
    }

    private void ApplySprite(Sprite s)
    {
        appliedSprite = s;

        if (s == null)
        {
            image.sprite = null;
            image.enabled = false;
            return;
        }

        image.sprite = s;
        image.enabled = true;
    }
}