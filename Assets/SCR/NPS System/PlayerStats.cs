using UnityEngine;

/// <summary>
/// Хранилище характеристик игрока: репутация и воля.
/// Статический класс — не вешается на GameObject, живёт на всю игру.
/// </summary>
public static class PlayerStats
{
    // --- Репутация: как игрока видят другие ---
    public static int Reputation = 0;

    // --- Воля: внутренняя сила/характер игрока ---
    public static int Will = 0;

    // Событие — если хочешь, чтобы UI сам обновлялся при изменении
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

    // Удобный метод — изменить оба сразу (для выбора в диалоге)
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

    // На случай новой игры — сбросить всё
    public static void Reset()
    {
        Reputation = 0;
        Will = 0;
        OnStatsChanged?.Invoke();
    }
}