using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    [SerializeField] private Transform locationsRoot;
    [SerializeField] private string startLocationId = "street";

    private readonly Dictionary<string, Location> locations = new();
    public Location Current { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (Transform child in locationsRoot)
        {
            var loc = child.GetComponent<Location>();
            if (loc != null) locations[loc.id] = loc;
        }
    }

    private void Start()
    {
        SetActiveLocation(startLocationId);
    }

    public void GoTo(string locationId)
    {
        if (!locations.ContainsKey(locationId))
        {
            Debug.LogError($"Локация '{locationId}' не найдена");
            return;
        }
        TransitionManager.Instance.PlayTransition(() => SetActiveLocation(locationId));
    }

    private void SetActiveLocation(string locationId)
    {
        foreach (var loc in locations.Values)
            loc.gameObject.SetActive(false);

        Current = locations[locationId];
        Current.gameObject.SetActive(true);

        GameState.Instance.visitedLocations.Add(locationId);
    }
}