using UnityEngine;

// Скрипт для предметов, которые можно подобрать
public class PickableItem : MonoBehaviour, IPickable
{
    // Метод интерфейса IPickable
    public void TryPickUp()
    {
        // Берём данные предмета из компонента CheckableItem
        CheckableItem checkable = GetComponent<CheckableItem>();
        if (checkable == null) return;

        ItemData currentData = checkable.data;
        if (currentData == null) return;

        // Добавляем предмет в инвентарь и удаляем объект со сцены
        Inventory.Instance.Add(currentData);
        Destroy(gameObject);
    }
}