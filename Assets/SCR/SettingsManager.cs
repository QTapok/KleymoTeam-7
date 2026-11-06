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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.8f);
        SetVolume(savedVolume);
    }

    public void SetVolume(float sliderValue)
    {
        float clamped = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        if (mainMixer != null)
            mainMixer.SetFloat("MusicVol", dB);

        PlayerPrefs.SetFloat(VOLUME_KEY, sliderValue);
        PlayerPrefs.Save();
    }

    public float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat(VOLUME_KEY, 0.8f);
    }
}