using UnityEngine;

public static class PlayerStats
{
    public static int Reputation = 0;
    public static int Will = 0;
    public static int Health = 100;
    public static int Hungry = 0;
    public static int Fatigue = 0;


    public static event System.Action OnStatsChanged;

    public static void AddReputation(int delta)
    {
        if (delta == 0) return;
        Reputation += delta;
        Debug.Log($"Репутация: {Reputation} ({delta:+#;-#;0})");
        OnStatsChanged?.Invoke();
    }

    public static void AddWill(int delta)
    {
        if (delta == 0) return;
        Will += delta;
        Debug.Log($"Воля: {Will} ({delta:+#;-#;0})");
        OnStatsChanged?.Invoke();
    }

    public static void Apply(int reputationDelta, int willDelta)
    {
        if (reputationDelta != 0)
        {
            Reputation += reputationDelta;
            Debug.Log($"Репутация: {Reputation} ({reputationDelta:+#;-#;0})");
        }
        if (willDelta != 0)
        {
            Will += willDelta;
            Debug.Log($"Воля: {Will} ({willDelta:+#;-#;0})");
        }
        if (reputationDelta != 0 || willDelta != 0)
            OnStatsChanged?.Invoke();
    }
    public static void AddFatigue(int delta)
    {
        if (delta == 0) return;
        Fatigue = Mathf.Clamp(Fatigue + delta, 0, 100);
        Debug.Log($"Усталость: {Fatigue} ({delta:+#;-#;0})");
        OnStatsChanged?.Invoke();
    }

    public static void AddHungry(int delta)
    {
        if (delta == 0) return;
        Hungry = Mathf.Clamp(Hungry + delta, 0, 100);
        Debug.Log($"Голод: {Hungry} ({delta:+#;-#;0})");
        OnStatsChanged?.Invoke();
    }

    public static void AddHealth(int delta)
    {
        if (delta == 0) return;
        Health = Mathf.Clamp(Health + delta, 0, 100);
        Debug.Log($"Здоровье: {Health} ({delta:+#;-#;0})");
        OnStatsChanged?.Invoke();
    }

    public static void Reset()
    {
        Reputation = 0;
        Will = 50;
        Health = 100;
        Hungry = 0;
        Fatigue = 0;
        OnStatsChanged?.Invoke();
    }

    public static void ForceNotify() => OnStatsChanged?.Invoke();
}