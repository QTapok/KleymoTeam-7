using System.Collections.Generic;
using UnityEngine;

public class DayRandom : MonoBehaviour
{
    public static DayRandom Instance { get; private set; }

    [Header("Наборы вариантов (по одному на каждую логическую локацию)")]
    [SerializeField] private List<LocationVariantSet> variantSets = new List<LocationVariantSet>();

    [Header("Стартовая локация на день")]
    [SerializeField] private string startLocationId = "corridor";

    [Header("Контейнер для инстансов")]
    [Tooltip("Сюда перетащи объект LocationsRoot из сцены")]
    [SerializeField] private Transform instancesParent;


    private readonly Dictionary<string, GameObject> _todayInstances = new();
    private readonly Dictionary<string, LocationVariantSet> _setsById = new();
    private readonly Dictionary<string, Location> locations = new();

    public string CurrentLocationId { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // ↓↓↓ ВОТ ЭТОГО НЕ ХВАТАЛО ↓↓↓
        // Заполняем словарь наборами вариантов при старте
        foreach (var set in variantSets)
        {
            if (set == null || string.IsNullOrEmpty(set.locationId)) continue;
            _setsById[set.locationId] = set;
        }
        // ↑↑↑ ВОТ ЭТОГО НЕ ХВАТАЛО ↑↑↑
    }

    private void Start()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("[DayRandom] TimeManager не найден!");
            return;
        }

        TimeManager.Instance.OnDayChanged += OnDayChanged;

        SpawnLocationsForToday();
        ShowLocation(startLocationId);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayChanged -= OnDayChanged;
    }

    private void OnDayChanged(int day)
    {
        ClearCurrentDayInstances();
        SpawnLocationsForToday();
        ShowLocation(startLocationId);
    }

    private void ClearCurrentDayInstances()
    {
        locations.Clear();
        foreach (var pair in _todayInstances)
        {
            if (pair.Value != null)
                Destroy(pair.Value);
        }
        _todayInstances.Clear();
        CurrentLocationId = null;
    }

    private void SpawnLocationsForToday()
    {
        foreach (var pair in _setsById)
        {
            var set = pair.Value;
            if (set.variants == null || set.variants.Count == 0)
            {
                Debug.LogWarning($"[DayRandom] Пустой набор вариантов для '{set.locationId}'");
                continue;
            }

            int idx = Random.Range(0, set.variants.Count);
            var prefab = set.variants[idx];
            if (prefab == null) continue;

            var instance = Instantiate(prefab, instancesParent);
            instance.SetActive(false);

            // Сохраняем компонент для старой логики
            var loc = instance.GetComponent<Location>();
            if (loc != null)
                locations[loc.id] = loc;
            else
                Debug.LogWarning($"[DayRandom] На префабе {prefab.name} нет компонента Location");

            _todayInstances[set.locationId] = instance;

            Debug.Log($"[DayRandom] День {TimeManager.Instance.CurrentDay}: '{set.locationId}' -> {prefab.name}");
        }
    }

    public void ShowLocation(string locationId)
    {
        if (!_todayInstances.TryGetValue(locationId, out var target) || target == null)
        {
            Debug.LogError($"[DayRandom] Нет инстанса на сегодня для локации '{locationId}'");
            return;
        }

        foreach (var pair in _todayInstances)
        {
            if (pair.Value != null)
                pair.Value.SetActive(pair.Key == locationId);
        }

        CurrentLocationId = locationId;
        Debug.Log($"[DayRandom] Показана локация '{locationId}'");
    }

    public GameObject GetCurrentLocationInstance()
    {
        return CurrentLocationId != null && _todayInstances.TryGetValue(CurrentLocationId, out var go)
            ? go
            : null;
    }
}