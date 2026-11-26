using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Ссылки")]
    [SerializeField] private ItemDataBase itemDatabase;
    [SerializeField] private string gameSceneName = "Game";

    [Header("Отладка")]
    [SerializeField] private bool verboseLog = true;

    public int CurrentSlot { get; private set; } = -1;
    public bool IsLoading { get; private set; } = false;

    private SaveData _pending;
    private bool _applyPending;

    private static string PathFor(int slot)
        => Path.Combine(Application.persistentDataPath, $"save_{slot}.json");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        //Автосейв в начале дня
        StartCoroutine(SubscribeAutoSave());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayChanged -= HandleDayChanged;
    }

    private IEnumerator SubscribeAutoSave()
    {
        while (TimeManager.Instance == null) yield return null;
        TimeManager.Instance.OnDayChanged += HandleDayChanged;
    }

    private void HandleDayChanged(int day)
    {
        if (IsLoading) return;
        if (CurrentSlot < 0) return;
        SaveCurrent();
    }
    //API
    public bool SlotExists(int slot)
        => slot >= 0 && slot <= 2 && File.Exists(PathFor(slot));

    public SaveData PeekSlot(int slot)
    {
        if (!SlotExists(slot)) return null;
        try
        {
            string json = File.ReadAllText(PathFor(slot));
            return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Save] PeekSlot({slot}) failed: {e.Message}");
            return null;
        }
    }

    public void DeleteSlot(int slot)
    {
        if (!SlotExists(slot)) return;
        File.Delete(PathFor(slot));
        if (CurrentSlot == slot) CurrentSlot = -1;
    }

    public int FindFreeSlot()
    {
        for (int i = 0; i < 3; i++) if (!SlotExists(i)) return i;
        return -1;
    }

    public void StartNewGame(int slot)
    {
        if (slot < 0 || slot > 2) { Debug.LogError($"[Save] Bad slot {slot}"); return; }

        DeleteSlot(slot);

        CurrentSlot = slot;

        _pending = BuildDefaultSave();
        _applyPending = true;
        IsLoading = true;

        DialogueFlags.ResetAll();

        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadFromSlot(int slot)
    {
        if (!SlotExists(slot)) { Debug.LogWarning($"[Save] Slot {slot} empty"); return; }
        var data = PeekSlot(slot);
        if (data == null) { Debug.LogError($"[Save] Slot {slot} unreadable"); return; }

        CurrentSlot = slot;
        _pending = data;
        _applyPending = true;
        IsLoading = true;

        SceneManager.LoadScene(gameSceneName);
    }

    public void SaveCurrent()
    {
        if (CurrentSlot < 0) { Debug.LogWarning("[Save] No slot selected"); return; }

        var data = CaptureCurrent();
        try
        {
            File.WriteAllText(PathFor(CurrentSlot), JsonUtility.ToJson(data, true));
            if (verboseLog) Debug.Log($"[Save] Slot {CurrentSlot} saved");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Save] Write failed: {e.Message}");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_applyPending) return;

        StartCoroutine(ApplyNextFrame());
    }

    private IEnumerator ApplyNextFrame()
    {
        yield return null;

        var data = _pending;
        _pending = null;
        _applyPending = false;

        if (data != null)
            ApplyData(data);

        IsLoading = false;
    }

    private void ApplyData(SaveData d)
    {
        ApplyLocations(d);

        if (TimeManager.Instance != null)
            TimeManager.Instance.SetTime(d.hour, d.day);

        PlayerStats.Reputation = d.reputation;
        PlayerStats.Will = d.will;
        PlayerStats.Health = d.health;
        PlayerStats.Hungry = d.hungry;
        PlayerStats.Fatigue = d.fatigue;
        PlayerStats.ForceNotify();

        if (Inventory.Instance != null)
        {
            Inventory.Instance.ClearAll();
            if (itemDatabase != null)
            {
                foreach (int id in d.inventoryItemIds)
                {
                    var item = itemDatabase.Get(id);
                    if (item != null) Inventory.Instance.Add(item);
                    else Debug.LogWarning($"[Save] Item id {id} not found in DB");
                }
            }
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.flags.Clear();
            GameState.Instance.visitedLocations.Clear();
            GameState.Instance.inventoryItems.Clear();
            foreach (var f in d.gameStateFlags) GameState.Instance.flags.Add(f);
            foreach (var l in d.visitedLocations) GameState.Instance.visitedLocations.Add(l);
            foreach (var i in d.gameStateItems) GameState.Instance.inventoryItems.Add(i);
        }

        DialogueFlags.ImportJson(d.dialogueFlagsJson);
    }

    private SaveData CaptureCurrent()
    {
        var d = new SaveData();

        if (TimeManager.Instance != null)
        {
            d.hour = TimeManager.Instance.CurrentHour;
            d.day = TimeManager.Instance.CurrentDay;
        }

        d.reputation = PlayerStats.Reputation;
        d.will = PlayerStats.Will;
        d.health = PlayerStats.Health;
        d.hungry = PlayerStats.Hungry;
        d.fatigue = PlayerStats.Fatigue;

        if (Inventory.Instance != null)
            foreach (var it in Inventory.Instance.items)
                if (it != null) d.inventoryItemIds.Add(it.id);

        if (GameState.Instance != null)
        {
            d.gameStateFlags = new List<string>(GameState.Instance.flags);
            d.visitedLocations = new List<string>(GameState.Instance.visitedLocations);
            d.gameStateItems = new List<string>(GameState.Instance.inventoryItems);
        }

        d.dialogueFlagsJson = DialogueFlags.ExportJson();

        CaptureLocations(d);

        return d;
    }

    private SaveData BuildDefaultSave()
    {
        var d = new SaveData
        {
            hour = 6,
            day = 1,
            reputation = 0,
            will = 0,
            health = 100,
            hungry = 0,
            fatigue = 0,
            dialogueFlagsJson = "",
            todayLocations = new List<LocationChoice>()
        };
        return d;
    }

    private void CaptureLocations(SaveData d)
    {
        var lm = LocRanMan.Instance;
        if (lm == null) return;

        d.currentLocationId = lm.CurrentLocationId ?? "";

        foreach (var kv in lm.GetTodayInstancesSnapshot())
        {
            if (kv.Value == null) continue;

            string locId = kv.Key;
            var variant = kv.Value.GetComponent<LocationVariant>();
            if (variant != null && !string.IsNullOrEmpty(variant.locationId))
                locId = variant.locationId;

            d.todayLocations.Add(new LocationChoice
            {
                locationId = locId,
                prefabName = kv.Value.name.Replace("(Clone)", "").Trim()
            });
        }
    }

    private void ApplyLocations(SaveData d)
    {
        var lm = LocRanMan.Instance;
        if (lm == null) return;

        lm.RestoreTodayInstances(d.todayLocations, d.currentLocationId);
    }
}