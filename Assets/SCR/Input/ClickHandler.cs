using UnityEngine;
using UnityEngine.InputSystem;

// Этот скрипт — центральный обработчик ввода.
// Он висит один раз в сцене и реагирует на клики мыши,
// а сами игровые объекты решают, что с этим кликом делать
// через интерфейсы.
public class ClickHandler : MonoBehaviour
{
    private void Update()
    {
        // Если по какой-то причине мышь недоступна — ничего не делаем
        if (Mouse.current == null) return;

        // ===== ЛЕВЫЙ КЛИК =====
        // Используется для осмотра объектов (подсказки / описание)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Получаем позицию мыши в мировых координатах
            Vector2 mouseWorldPos =
                Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Пускаем луч "в точку" — проверяем, есть ли коллайдер под курсором
            RaycastHit2D hit =
                Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            // Если под курсором есть объект
            // и он поддерживает интерфейс ICheckable —
            // показываем его описание
            if (hit.collider != null &&
                hit.collider.TryGetComponent<ICheckable>(out var checkable))
            {
                checkable.TellAbout();
            }
        }

        // ===== ПРАВЫЙ КЛИК =====
        // Контекстное действие: подбор, выбор или использование предмета
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // Позиция мыши в мировых координатах
            Vector2 mouseWorldPos =
                Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Проверяем, попали ли мы в объект сцены
            RaycastHit2D hit =
                Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            // Если ни во что не попали — выходим
            if (hit.collider == null) return;

            // 1. Проверяем, можно ли объект подобрать
            // (ножницы, ключ, записка с кодом)
            if (hit.collider.TryGetComponent<IPickable>(out var pickable))
            {
                pickable.TryPickUp();

                // Важно: если предмет подобран,
                // дальше этот клик обрабатывать не нужно
                return;
            }

            // 2. Если объект нельзя подобрать,
            // проверяем, можно ли использовать выбранный предмет
            if (hit.collider.TryGetComponent<IUsable>(out var usable))
            {
                // Использовать можно только если в инвентаре
                // выбран какой-то предмет
                if (Inventory.Instance.SelectedItem != null)
                {
                    usable.TryUse(Inventory.Instance.SelectedItem);
                }
            }
        }
    }
}
