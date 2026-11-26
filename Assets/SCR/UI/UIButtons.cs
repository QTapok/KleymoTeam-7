using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Панель инвентаря")]
    [SerializeField] private RectTransform InventoryPanel;

    [Tooltip("Панель настроек/паузы")]
    [SerializeField] private RectTransform SettingsPanel;

    [Tooltip("Кнопки")]
    [SerializeField] private Button openInventoryButton;
    [SerializeField] private Button openSettingsButton;

    [Header("Клавиши")]
    [SerializeField] private Key inventoryKey = Key.E;
    [SerializeField] private Key settingsKey = Key.Escape;

    [Header("Анимация")]
    [SerializeField] private float animationSpeed = 8f;

    [Header("Инвентарь — позиции")]
    [SerializeField] private Vector2 InventoryHiddenPosition;
    [SerializeField] private Vector2 InventoryShownPosition;

    [Header("Настройки — позиции (выдвижение сверху вниз)")]
    [SerializeField] private Vector2 SettingsHiddenPosition;
    [SerializeField] private Vector2 SettingsShownPosition;

    //[Header("Убираемые обьекты")]
    //[SerializeField] private GameObject One;

    //public bool isDialogueOpen = false;
    private bool isInventoryOpen = false;
    private bool isSettingsOpen = false;

    private Coroutine inventoryCoroutine;
    private Coroutine settingsCoroutine;

    public static UIButtons Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    private void Start()
    {
        InventoryPanel.anchoredPosition = InventoryHiddenPosition;
        InventoryPanel.gameObject.SetActive(true);
        SettingsPanel.anchoredPosition = SettingsHiddenPosition;
        SettingsPanel.gameObject.SetActive(true);

        //One.SetActive(false);

        if (openInventoryButton != null)
            openInventoryButton.onClick.AddListener(ToggleInventory);

        if (openSettingsButton != null)
            openSettingsButton.onClick.AddListener(ToggleSettings);

        

    }

    private void Update()
    {
        //if (One == null) isDialogueOpen = false;
        //else if (One != null && isDialogueOpen == false) isDialogueOpen = true;

        if (Keyboard.current == null) return;

        if (Keyboard.current[inventoryKey].wasPressedThisFrame)
            ToggleInventory();

        if (Keyboard.current[settingsKey].wasPressedThisFrame)
            ToggleSettings();

    }

    //Инвентарь
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryCoroutine != null)
            StopCoroutine(inventoryCoroutine);

        Vector2 target = isInventoryOpen ? InventoryShownPosition : InventoryHiddenPosition;
        inventoryCoroutine = StartCoroutine(AnimatePanel(InventoryPanel, target));
    }

    //Настройки и пауза
    public void ToggleSettings()
    {
        //One.SetActive(false);
        isSettingsOpen = !isSettingsOpen;

        if (settingsCoroutine != null)
            StopCoroutine(settingsCoroutine);

        Vector2 target = isSettingsOpen ? SettingsShownPosition : SettingsHiddenPosition;
        settingsCoroutine = StartCoroutine(AnimatePanel(SettingsPanel, target));

        Time.timeScale = isSettingsOpen ? 0f : 1f;

        if (openSettingsButton != null)
            openSettingsButton.interactable = !isSettingsOpen;
    }

    private IEnumerator AnimatePanel(RectTransform panel, Vector2 targetPos)
    {
        while (Vector2.Distance(panel.anchoredPosition, targetPos) > 0.5f)
        {
            panel.anchoredPosition = Vector2.Lerp(
                panel.anchoredPosition,
                targetPos,
                Time.unscaledDeltaTime * animationSpeed
            );
            yield return null;
        }
        panel.anchoredPosition = targetPos;
    }

    private void OnDestroy()
    {
        if (openInventoryButton != null)
            openInventoryButton.onClick.RemoveListener(ToggleInventory);

        if (openSettingsButton != null)
            openSettingsButton.onClick.RemoveListener(ToggleSettings);
    }
    public void CloseSettings()
    {
        

        if (!isSettingsOpen) return;

        isSettingsOpen = false;

        if (settingsCoroutine != null)
            StopCoroutine(settingsCoroutine);

        settingsCoroutine = StartCoroutine(AnimatePanel(SettingsPanel, SettingsHiddenPosition));

        Time.timeScale = 1f;

        //One.SetActive(isDialogueOpen);

        if (openSettingsButton != null)
            openSettingsButton.interactable = true;
    }

    public void OnSaveClicked()
    {
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.SaveCurrent();
    }

    public void OnMenuClicked(bool autoSave)
    {
        if (autoSave && SaveManager.Instance != null &&
            SaveManager.Instance.CurrentSlot >= 0 &&
            !SaveManager.Instance.IsLoading)
        {
            SaveManager.Instance.SaveCurrent();
        }
        SceneManager.LoadScene("MainMenu");
    }
}