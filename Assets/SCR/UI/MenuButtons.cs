using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Кнопки")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button setButton;

    [Header("Кнопка открытия меню")]
    [Tooltip("Кнопка на HUD, которая открывает меню паузы")]
    [SerializeField] private Button openSettingsButton;

    [Tooltip("Настройки звука")]
    [SerializeField] private GameObject CanselButton;
    [SerializeField] private GameObject Slider;
    private void Start()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResume);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        if (setButton != null)
            setButton.onClick.AddListener(OnSet);
    }

    private void OnResume()
    {
        // Закрываем меню через тот же UIButtons, что и открывал
        UIButtons.Instance?.CloseSettings();
    }

    private void OnQuit()
    {
        SceneManager.LoadScene(0);
    }
    private void OnSet()
    {
        Slider.SetActive(true);
        CanselButton.SetActive(true);
        resumeButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        setButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResume);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuit);

        if (setButton != null)
            setButton.onClick.RemoveListener(OnSet);

    }
}