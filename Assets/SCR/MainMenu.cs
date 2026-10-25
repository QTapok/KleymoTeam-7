using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject StartButton;
    [SerializeField] public GameObject ExitButton;
    [SerializeField] public GameObject SettingsButton;
    [SerializeField] public GameObject CancelButton;
    [SerializeField] public GameObject Slider;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenSettings()
    {
        StartButton.SetActive(false);
        ExitButton.SetActive(false);
        SettingsButton.SetActive(false);
        Slider.SetActive(true);
        CancelButton.SetActive(true);
    }
    public void CloseSettings()
    {
        StartButton.SetActive(true);
        ExitButton.SetActive(true);
        SettingsButton.SetActive(true);
        Slider.SetActive(false);
        CancelButton.SetActive(false);
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
    
}