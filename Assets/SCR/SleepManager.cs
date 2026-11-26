using UnityEngine;

public class SleepManager : MonoBehaviour
{
    public static SleepManager Instance { get; private set; }

    [Header("Настройки сна")]
    [Tooltip("Час, во сколько игрок просыпается (например, 6 утра)")]
    [SerializeField] private int wakeUpHour = 6;
    [Tooltip("На сколько восстанавливается усталость за час сна")]
    [SerializeField] private int fatigueRecoveryPerHour = 10;
    [Tooltip("На сколько увеличивается усталость за час бодрствования НОЧЬЮ")]
    [SerializeField] private int nightFatigueIncrease = 5;
    [Tooltip("На сколько увеличивается усталость за час бодрствования ДНЕМ")]
    [SerializeField] private int dayFatigueIncrease = 1;

    [Header("Границы ночи")]
    [SerializeField] private int nightStartHour = 22;
    [SerializeField] private int nightEndHour = 6;

    public bool IsSleeping { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged += OnHourChanged;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= OnHourChanged;
    }

    // Вызывается, когда игрок кликает на кровать
    public void GoToBed()
    {
        if (IsSleeping) return;
        IsSleeping = true;
        Debug.Log("Игрок лег спать... Zzz");

        int hoursToSkip = GetHoursUntilMorning();

        // Восстановление характеристик во сне
        PlayerStats.AddFatigue(-hoursToSkip * fatigueRecoveryPerHour);
        PlayerStats.AddHealth(hoursToSkip * 2); // Опционально: сон немного лечит

        // Перемотка времени до утра (TimeManager сам вызовет смену дня)
        TimeManager.Instance.Advance(hoursToSkip);

        IsSleeping = false;
        Debug.Log("Доброе утро! Игрок проснулся.");
    }

    // Принудительный сон (вызывается из DailySchedule при штрафе или при обмороке)
    public void KnockOut()
    {
        if (IsSleeping) return;
        Debug.Log("Игрок потерял сознание и уснул.");

        IsSleeping = true;
        int hoursToSkip = GetHoursUntilMorning();

        // При обмороке восстановление хуже, чем в кровати
        PlayerStats.AddFatigue(-hoursToSkip * fatigueRecoveryPerHour / 2);

        TimeManager.Instance.Advance(hoursToSkip);
        IsSleeping = false;
    }

    private void OnHourChanged(int hour, int day)
    {
        if (IsSleeping) return; // Если спит, усталость не растет

        // 1. Проверка на отключение от критической усталости
        if (PlayerStats.Fatigue >= 100)
        {
            Debug.Log("Игрок вырубился от усталости!");
            KnockOut();
            return;
        }

        // 2. Почасовой рост усталости
        bool isNight = hour >= nightStartHour || hour < nightEndHour;
        int fatigueDelta = isNight ? nightFatigueIncrease : dayFatigueIncrease;

        PlayerStats.AddFatigue(fatigueDelta);
    }

    private int GetHoursUntilMorning()
    {
        int currentHour = TimeManager.Instance.CurrentHour;
        if (currentHour >= wakeUpHour)
        {
            return (24 - currentHour) + wakeUpHour;
        }
        return wakeUpHour - currentHour;
    }
}