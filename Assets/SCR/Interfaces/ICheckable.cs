using UnityEngine;

// Интерфейс для объектов, которые можно "осмотреть"
// и получить текстовую подсказку
public interface ICheckable
{
    // Метод, который вызывается при осмотре объекта
    void TellAbout();
}
