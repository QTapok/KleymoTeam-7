using UnityEngine;

// ScriptableObject — контейнер для данных предмета.
// Хранит информацию отдельно от логики и объектов сцены.
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    // Уникальный идентификатор предмета
    public int id;

    // Название предмета, которое видит игрок
    public string title;

    // Текстовое описание предмета
    [TextArea]
    public string description;

    // Иконка предмета для UI и курсора
    public Sprite icon;
}