using UnityEngine;

public class DayTimeUI : MonoBehaviour
{
    [Header("Стрелки")]
    [Tooltip("Часовая стрелка")]
    [SerializeField] private RectTransform hourHand;

    [Header("Направление вращения")]
    [SerializeField] private float rotationDirection = 1f;

    [Header("Сектор индикатора")]
    [SerializeField] private float leftAngle = -105f;
    [SerializeField] private float rightAngle = 105f;
    [SerializeField] private float angleStep = 15f;//шагг

    [Header("Часовые границы")]
    [SerializeField] private int sunriseHour = 6;
    [SerializeField] private int sunsetHour = 20;

    [Range(0, 1)]
    [SerializeField] private int nightHold = 0;
    private int PositionCount => Mathf.RoundToInt((leftAngle - rightAngle) / angleStep) + 1;


    private void Start()
    {
        if (TimeManager.Instance == null) return;
        TimeManager.Instance.OnTimeChanged += UpdateIndicator;
        UpdateIndicator(TimeManager.Instance.CurrentHour, TimeManager.Instance.CurrentDay);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= UpdateIndicator;
    }

    private void UpdateIndicator(int hour, int day)
    {
        if (hourHand == null) return;

        float angle = HourToAngle(hour);
        hourHand.localRotation = Quaternion.Euler(0, 0, angle * rotationDirection);
    }

    private float HourToAngle(int hour)
    {
        int positions = PositionCount;
        int dayLength = sunsetHour - sunriseHour;
        if (dayLength <= 0) return leftAngle;

        bool isNight = hour < sunriseHour || hour > sunsetHour;
        if (isNight)
            return nightHold == 0 ? rightAngle : leftAngle;

        float t = (float)(hour - sunriseHour) / dayLength;

        int pos = Mathf.Clamp(Mathf.RoundToInt(t * (positions - 1)), 0, positions - 1);

        return leftAngle - pos * angleStep;
    }
}