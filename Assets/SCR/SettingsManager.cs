using UnityEngine;
using UnityEngine.Audio;

using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private AudioMixer mainMixer;
    private const string VOLUME_KEY = "MasterVolume";

    private void Awake()
    {
        // Реализация Singleton: если экземпляр уже существует, уничтожить дубликат
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Ключевая строка: не уничтожать при переходе между сценами
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // При запуске игры прочитать сохранённое значение громкости, по умолчанию 0.8
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.8f);
        SetVolume(savedVolume);
    }

    // Метод, вызываемый слайдером
    public void SetVolume(float sliderValue)
    {
        // 1. Применить к AudioMixer (с преобразованием в децибелы)
        float clamped = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        if (mainMixer != null)
            mainMixer.SetFloat("MusicVol", dB);

        // 2. Сохранить в PlayerPrefs для использования в следующей сцене или при следующем запуске
        PlayerPrefs.SetFloat(VOLUME_KEY, sliderValue);
        PlayerPrefs.Save();
    }

    // Для удобства чтения слайдером при входе в новую сцену
    public float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat(VOLUME_KEY, 0.8f);
    }
}
