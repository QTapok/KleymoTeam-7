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
        // Если SettingsManager ещё нет — создаём (на случай запуска сразу со второй сцены)
        if (SettingsManager.Instance == null)
        {
            GameObject managerObj = new GameObject("SettingsManager");
            managerObj.AddComponent<SettingsManager>();
        }

        // 1. Синхронизируем положение ползунка с сохранённым значением
        float saved = PlayerPrefs.GetFloat("MasterVolume", 0.8f);

        // Важно: временно отписываемся, чтобы установка value не вызвала SetVolume
        slider.onValueChanged.RemoveListener(SettingsManager.Instance.SetVolume);
        slider.value = saved;
        slider.onValueChanged.AddListener(SettingsManager.Instance.SetVolume);
    }

    private void OnDisable()
    {
        // Отписываемся при выключении, чтобы не копить слушатели
        if (SettingsManager.Instance != null && slider != null)
            slider.onValueChanged.RemoveListener(SettingsManager.Instance.SetVolume);
    }
}