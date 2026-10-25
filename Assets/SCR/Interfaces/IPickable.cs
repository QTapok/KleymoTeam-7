using UnityEngine;

// Интерфейс для объектов, которые можно подобрать
public interface IPickable
{
    // Метод вызывается, когда игрок подбирает объект
    void TryPickUp();
}