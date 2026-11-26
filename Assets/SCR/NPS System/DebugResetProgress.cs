//F8 полный сброс прогресса
//F7 вывести текущее состояние в консоль
//F6 передвинуть стрелку часов

using UnityEngine;
using UnityEngine.InputSystem;

public class DebugResetProgress : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null) return;

        
        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {
            GameProgress.ResetAll();
        }

        
        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            string flags = PlayerPrefs.GetString("dlg.save", "<empty>");
            Debug.Log(
                $"Флаги: {flags}\n" +
                $"Репутация: {PlayerStats.Reputation}, Воля: {PlayerStats.Will}\n" +
                $"Предметов в инвентаре: {(Inventory.Instance != null ? Inventory.Instance.items.Count : -1)}");
        }
        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            TimeManager.Instance.Advance(1);
        }

    }
}