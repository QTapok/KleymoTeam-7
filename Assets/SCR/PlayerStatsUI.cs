using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Слайдеры")]
    [SerializeField] private Slider reputationSlider;
    [SerializeField] private Slider willSlider;

    [Header("Настройки шкал")]
    [SerializeField] private int reputationMax = 100;
    [SerializeField] private int willMax = 100;
    [SerializeField] private bool clampToZero = true;

    private void OnEnable()
    {
        PlayerStats.OnStatsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerStats.OnStatsChanged -= Refresh;
    }

    private void Refresh()
    {
        UpdateSlider(reputationSlider, PlayerStats.Reputation, reputationMax);
        UpdateSlider(willSlider, PlayerStats.Will, willMax);
    }

    private void UpdateSlider(Slider slider, int value, int max)
    {
        if (slider == null) return;

        slider.minValue = clampToZero ? 0 : -max;
        slider.maxValue = max;
        slider.value = Mathf.Clamp(value, (int)slider.minValue, max);
    }
}