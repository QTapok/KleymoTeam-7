using System.Collections.Generic;
using UnityEngine;

public class LocRanMan : MonoBehaviour
{
    public static LocRanMan Instance { get; private set; }

    [Header("Настройки случайных локаций")]
    [SerializeField] private List<LocationVariantSet> variantSets = new List<LocationVariantSet>();
    [SerializeField] private Transform locationsRoot;

    [Header("Стартовая локация")]
    [SerializeField] private string startLocationId = "street";

    private readonly Dictionary<string, GameObject> _todayInstances = new();
    private readonly Dictionary<string, LocationVariantSet> _setsById = new();

    public string CurrentLocationId { get; private set; }
    public GameObject Current => CurrentLocationId != null && _todayInstances.TryGetValue(CurrentLocationId, out var go) ? go : null;

    public event System.Action<string, string> OnLocationChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var set in variantSets)
        {
            if (set == null || string.IsNullOrEmpty(set.locationId)) continue;
            _setsById[set.locationId] = set;
        }
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayChanged += OnDayChanged;

        if (SaveManager.Instance != null && SaveManager.Instance.IsLoading)
            return;

        SpawnLocationsForToday();
        SetActiveLocation(startLocationId, false);
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
        SetActiveLocation(startLocationId, false);
    }

    private void ClearCurrentDayInstances()
    {
        foreach (var pair in _todayInstances)
            if (pair.Value != null) Destroy(pair.Value);
        _todayInstances.Clear();
        CurrentLocationId = null;
    }

    private void SpawnLocationsForToday()
    {
        foreach (var pair in _setsById)
        {
            var set = pair.Value;
            if (set.variants == null || set.variants.Count == 0) continue;

            int idx = Random.Range(0, set.variants.Count);
            var prefab = set.variants[idx];
            if (prefab == null) continue;

            var instance = Instantiate(prefab, locationsRoot);
            instance.SetActive(false);
            _todayInstances[set.locationId] = instance;
        }
    }

    public void GoTo(string locationId)
    {
        if (!_todayInstances.ContainsKey(locationId))
        {
            Debug.LogError($"[LocationManager] Локация '{locationId}' не найдена.");
            return;
        }

        if (TransitionManager.Instance != null)
            TransitionManager.Instance.PlayTransition(() => SetActiveLocation(locationId, true));
        else
            SetActiveLocation(locationId, true);
    }

    private void SetActiveLocation(string locationId, bool fireEvent)
    {
        if (!_todayInstances.TryGetValue(locationId, out var target) || target == null)
            return;

        string oldId = CurrentLocationId;

        foreach (var pair in _todayInstances)
            if (pair.Value != null) pair.Value.SetActive(pair.Key == locationId);

        CurrentLocationId = locationId;

        if (fireEvent)
        {
            if (GameState.Instance != null)
                GameState.Instance.visitedLocations.Add(locationId);
            OnLocationChanged?.Invoke(oldId, locationId);
        }
    }

    public Dictionary<string, GameObject> GetTodayInstancesSnapshot() => _todayInstances;

    public void RestoreTodayInstances(List<LocationChoice> saved, string currentId)
    {
        ClearCurrentDayInstances();

        if (saved == null || saved.Count == 0)
        {
            SpawnLocationsForToday();
            SetActiveLocation(startLocationId, false);
            return;
        }

        foreach (var lc in saved)
        {
            if (!_setsById.TryGetValue(lc.locationId, out var set) || set.variants == null) continue;

            GameObject prefab = null;
            foreach (var v in set.variants)
                if (v != null && v.name == lc.prefabName) { prefab = v; break; }

            if (prefab == null && set.variants.Count > 0)
            {
                Debug.LogWarning($"[LocRan] Prefab '{lc.prefabName}' not found for '{lc.locationId}', fallback to variants[0]");
                prefab = set.variants[0];
            }
            if (prefab == null) continue;

            var instance = Instantiate(prefab, locationsRoot);
            instance.name = prefab.name;
            instance.SetActive(false);
            _todayInstances[lc.locationId] = instance;
        }

        string target = string.IsNullOrEmpty(currentId) ? startLocationId : currentId;
        if (_todayInstances.ContainsKey(target))
            SetActiveLocation(target, false);
        else if (_todayInstances.Count > 0)
            SetActiveLocation(new List<string>(_todayInstances.Keys)[0], false);
    }
}