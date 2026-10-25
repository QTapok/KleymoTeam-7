using UnityEngine;

// »нтерфейс дл€ объектов, которые можно использовать
// с предметом из инвентар€
public interface IUsable
{
    // otherData Ч данные предмета, который игрок пытаетс€ применить
    void TryUse(ItemData otherData);
}