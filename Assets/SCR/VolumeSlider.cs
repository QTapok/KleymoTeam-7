using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (SettingsManager.Instance == null)
        {
            GameObject managerObj = new GameObject("SettingsManager");
            managerObj.AddComponent<SettingsManager>();
        }

        float saved = PlayerPrefs.GetFloat("MasterVolume", 0.8f);

        slider.onValueChanged.RemoveListener(SettingsManager.Instance.SetVolume);
        slider.value = saved;
        slider.onValueChanged.AddListener(SettingsManager.Instance.SetVolume);
    }

    private void OnDisable()
    {
        if (SettingsManager.Instance != null && slider != null)
            slider.onValueChanged.RemoveListener(SettingsManager.Instance.SetVolume);
    }
}