using UnityEngine;

public class Bed : MonoBehaviour
{
    // Если кровать - это 3D объект с коллайдером
    private void OnMouseDown()
    {
        TrySleep();
    }

    // Если вы используете UI кнопку, повесьте этот метод на событие OnClick
    public void TrySleep()
    {
        if (SleepManager.Instance == null)
        {
            Debug.LogError("SleepManager не найден на сцене!");
            return;
        }

        if (SleepManager.Instance.IsSleeping)
        {
            Debug.Log("Игрок уже спит.");
            return;
        }

        SleepManager.Instance.GoToBed();
    }
}