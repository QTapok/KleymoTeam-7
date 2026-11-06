using UnityEngine;

public static class GameProgress
{
    ///Полный сброс
    public static void ResetAll()
    {
        //Флаги диалогов
        DialogueFlags.ResetAll();
        //Репутация и воля
        PlayerStats.Reset();
        //Прочее
        Debug.Log("[GameProgress] Прогресс сброшен.");
    }
}