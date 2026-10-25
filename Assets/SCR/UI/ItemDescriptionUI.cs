using TMPro;
using UnityEngine;

//Этот скрипт управляет UI-панелью с текстом описания предметов
public class ItemDescriptionUI : MonoBehaviour
{
    // Статичная ссылка на экземпляр скрипта (Singleton)
    public static ItemDescriptionUI Instance;

    // Панель, которая служит фоном для текста
    [SerializeField] private GameObject panelText;

    // Текстовое поле TextMeshPro для вывода описания
    [SerializeField] private TMP_Text descriptionText;

    // Флаг, показывающий, отображается ли сейчас текст
    private bool isShowing = false;

    // Время (в секундах), через которое окно будет скрыто
    public float timeToDisable = 3f;

    // Текущий таймер скрытия
    private float tmpTimeToDisable;

    private void Awake()
    {
        // Реализация Singleton:
        // если экземпляр уже существует — удаляем дубликат
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Инициализируем таймер
        tmpTimeToDisable = timeToDisable;

        // Скрываем панель при старте сцены
        if (panelText != null)
            panelText.SetActive(false);
    }

    private void Update()
    {
        // Если текст сейчас не отображается — ничего не делаем
        if (!isShowing) return;

        // Уменьшаем таймер с учётом времени кадра
        tmpTimeToDisable -= Time.deltaTime;

        // Если время вышло — скрываем окно
        if (tmpTimeToDisable <= 0f)
            Hide();
    }

    // Метод для показа текста описания
    public void Show(ItemData item)
    {
        // Если данные не переданы — просто скрываем окно
        if (item == null)
        {
            Hide();
            return;
        }

        // Защита от отсутствующих ссылок в инспекторе
        if (panelText == null || descriptionText == null) return;

        // Активируем отображение
        isShowing = true;

        // Сбрасываем таймер скрытия
        tmpTimeToDisable = timeToDisable;

        // Показываем панель и выводим текст
        panelText.SetActive(true);
        descriptionText.text = item.description;
    }

    // Скрытие панели с текстом
    private void Hide()
    {
        isShowing = false;

        if (panelText != null)
            panelText.SetActive(false);
    }
}

