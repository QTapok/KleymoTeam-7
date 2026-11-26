using System;
using System.Collections.Generic;

[Serializable]
public class LocationChoice
{
    public string locationId;
    public string prefabName;
}

[Serializable]
public class SaveData
{
    public int version = 1;

    //Время
    public int hour = 6;
    public int day = 1;

    //Статистика
    public int reputation = 0;
    public int will = 0;
    public int health = 100;
    public int hungry = 0;
    public int fatigue = 0;

    //Инвентарь
    public List<int> inventoryItemIds = new List<int>();

    // GameState
    public List<string> gameStateFlags = new List<string>();
    public List<string> visitedLocations = new List<string>();
    public List<string> gameStateItems = new List<string>();

    //Диалоги
    public string dialogueFlagsJson = "";

    //Локации
    public string currentLocationId = "";
    public List<LocationChoice> todayLocations = new List<LocationChoice>();
}