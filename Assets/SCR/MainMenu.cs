using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject StartButton;
    [SerializeField] public GameObject ExitButton;
    [SerializeField] public GameObject SettingsButton;
    [SerializeField] public GameObject CancelButton;
    [SerializeField] public GameObject Slider;
    [SerializeField] public GameObject Panel;

    [Header("Слоты сохранения")]
    [SerializeField] private SaveSlotView[] slotViews = new SaveSlotView[3];

    private void Start()
    {
        RefreshSlots();
    }
    public void StartGame()
    {
        Debug.Log("Persistent data path: " + Application.persistentDataPath);
        StartButton.SetActive(false);
        ExitButton.SetActive(false);
        SettingsButton.SetActive(false);
        Slider.SetActive(false);
        CancelButton.SetActive(false);
        Panel.SetActive(true);
    }
    public void OpenSettings()
    {
        StartButton.SetActive(false);
        ExitButton.SetActive(false);
        SettingsButton.SetActive(false);
        Slider.SetActive(true);
        CancelButton.SetActive(true);
        Panel.SetActive(false);
    }
    public void CloseSettings()
    {
        StartButton.SetActive(true);
        ExitButton.SetActive(true);
        SettingsButton.SetActive(true);
        Slider.SetActive(false);
        CancelButton.SetActive(false);
        Panel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Выход из игры");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private void RefreshSlots()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[MainMenu] SaveManager.Instance is null. " +
                           "SaveManager должен быть в сцене MainMenu.");
            return;
        }

        for (int i = 0; i < slotViews.Length && i < 3; i++)
        {
            if (slotViews[i] == null) continue;

            var data = SaveManager.Instance.PeekSlot(i);

            slotViews[i].Bind(i, data,
                onNewGame: slot => {
                    SaveManager.Instance.StartNewGame(slot);
                },
                onLoad: slot => {
                    SaveManager.Instance.LoadFromSlot(slot);
                },
                onDelete: slot => {
                    SaveManager.Instance.DeleteSlot(slot);
                    RefreshSlots(); // Обновляем карточки после удаления
                });
        }
    }

    public void OnNewGame()
    {
        if (SaveManager.Instance == null) return;

        int slot = SaveManager.Instance.FindFreeSlot();
        if (slot < 0) { Debug.LogWarning("[MainMenu] No free slot"); return; }

        SaveManager.Instance.StartNewGame(slot);
    }
}