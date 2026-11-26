using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Слайдеры")]
    [SerializeField] private Slider reputationSlider;
    [SerializeField] private Slider willSlider;
    [SerializeField] private Slider hungrySlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider fatigueSlider;

    [Header("Настройки шкал")]
    [SerializeField] private int reputationMax = 100;
    [SerializeField] private int willMax = 100;
    [SerializeField] private int healthMax = 100;
    [SerializeField] private int fatigueMax = 100;
    [SerializeField] private int hungryMax = 100;
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
        UpdateSlider(hungrySlider, PlayerStats.Hungry, hungryMax);
        UpdateSlider(healthSlider, PlayerStats.Health, healthMax);
        UpdateSlider(fatigueSlider, PlayerStats.Fatigue, fatigueMax);

    }

    private void UpdateSlider(Slider slider, int value, int max)
    {
        if (slider == null) return;

        slider.minValue = clampToZero ? 0 : -max;
        slider.maxValue = max;
        slider.value = Mathf.Clamp(value, (int)slider.minValue, max);
    }
}