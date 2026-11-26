using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverWatcher : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Имя сцены главного меню (должна быть добавлена в Build Settings)")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Tooltip("Опционально: UI панель с надписью 'Вы проиграли'")]
    [SerializeField] private GameObject gameOverUIPanel;

    private void Start()
    {
        PlayerStats.OnStatsChanged += CheckGameOver;
        CheckGameOver(); // Проверка на случай, если игрок уже мертв при загрузке
    }

    private void OnDestroy()
    {
        PlayerStats.OnStatsChanged -= CheckGameOver;
    }

    private void CheckGameOver()
    {
        // Health <= 0 (Смерть)
        // Hungry >= 100 (Истощение от голода, т.к. в PlayerStats шкала идет до 100)
        // Fatigue >= 100 (Критическая усталость)

        bool isDead = PlayerStats.Health <= 0;
        bool isStarving = PlayerStats.Hungry >= 100;
        bool isExhausted = PlayerStats.Fatigue >= 100;

        // ПРИМЕЧАНИЕ: Если по вашей задумке 0 - это критическое значение для голода/усталости, 
        // замените >= 100 на <= 0 в переменных isStarving и isExhausted.

        if (isDead || isStarving || isExhausted)
        {
            TriggerGameOver(isDead, isStarving, isExhausted);
        }
    }

    private void TriggerGameOver(bool dead, bool starved, bool exhausted)
    {
        Debug.LogWarning("=== GAME OVER ===");
        if (dead) Debug.Log("Причина: Здоровье упало до 0");
        if (starved) Debug.Log("Причина: Критический голод (100)");
        if (exhausted) Debug.Log("Причина: Критическая усталость (100)");

        PlayerStats.OnStatsChanged -= CheckGameOver;

        if (gameOverUIPanel != null)
            gameOverUIPanel.SetActive(true);

        Invoke(nameof(LoadMainMenu), 2.5f);
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}