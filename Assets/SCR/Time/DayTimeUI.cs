using UnityEngine;

public class DayTimeUI : MonoBehaviour
{
    [Header("Стрелки")]
    [Tooltip("Часовая стрелка. Pivot должен быть внизу (0.5, 0)")]
    [SerializeField] private RectTransform hourHand;

    [Tooltip("Минутная стрелка. Pivot должен быть внизу (0.5, 0)")]
    [SerializeField] private RectTransform minuteHand;

    [Header("Направление вращения")]
    [Tooltip("Если стрелки крутятся не в ту сторону — поставь -1")]
    [SerializeField] private float rotationDirection = -1f;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged += UpdateClock;
            // Сразу выставляем текущее время
            UpdateClock(TimeManager.Instance.CurrentHour,
                        TimeManager.Instance.CurrentMinute,
                        TimeManager.Instance.CurrentDay);
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= UpdateClock;
    }

    private void UpdateClock(int hour, int minute, int day)
    {
        // Приводим час к 12-часовому формату (0..11)
        int hour12 = hour % 12;

        // Угол часовой стрелки: 30° за час + 0.5° за минуту
        float hourAngle = (hour12 * 30f) + (minute * 0.5f);

        // Угол минутной стрелки: 6° за минуту
        float minuteAngle = minute * 6f;

        // Применяем вращение вокруг оси Z (для 2D UI)
        // rotationDirection = -1, если стрелки должны идти по часовой стрелке
        if (hourHand != null)
            hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle * rotationDirection);

        if (minuteHand != null)
            minuteHand.localRotation = Quaternion.Euler(0, 0, minuteAngle * rotationDirection);
    }
}
