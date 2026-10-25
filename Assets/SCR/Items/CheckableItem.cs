using UnityEngine;

// Этот скрипт добавляет объекту возможность
// показывать описание при осмотре
public class CheckableItem : MonoBehaviour, ICheckable
{
    // Данные предмета (ScriptableObject)
    public ItemData data;

    // Реализация метода интерфейса ICheckable
    public void TellAbout()
    {
        // Если данные не назначены — ничего не делаем
        if (data == null) return;

        // Передаём данные в UI для отображения текста
        ItemDescriptionUI.Instance.Show(data);
    }
}
