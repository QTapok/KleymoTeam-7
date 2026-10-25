using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public HashSet<string> visitedLocations = new();
    public HashSet<string> flags = new();
    public HashSet<string> inventoryItems = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasFlag(string flag) => flags.Contains(flag);
    public void SetFlag(string flag) => flags.Add(flag);
    public bool HasItem(string id) => inventoryItems.Contains(id);
    public void AddItem(string id) => inventoryItems.Add(id);
}