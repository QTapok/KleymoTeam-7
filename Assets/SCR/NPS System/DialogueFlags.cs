using System.Collections.Generic;
using UnityEngine;

public static class DialogueFlags
{
    private const string SaveKey = "dlg.save";

    [System.Serializable]
    private class Data
    {
        public List<string> flags = new List<string>();
        public List<string> usedChoices = new List<string>();
    }

    private static Data data;

    private static Data Store
    {
        get
        {
            if (data != null) return data;

            string json = PlayerPrefs.GetString(SaveKey, "");
            if (string.IsNullOrEmpty(json))
            {
                data = new Data();
            }
            else
            {
                try { data = JsonUtility.FromJson<Data>(json) ?? new Data(); }
                catch { data = new Data(); }
            }
            return data;
        }
    }

    private static void Save()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(Store));
        PlayerPrefs.Save();
    }
    public static bool Has(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return false;
        return Store.flags.Contains(flag);
    }

    public static void Set(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;
        if (!Store.flags.Contains(flag)) { Store.flags.Add(flag); Save(); }
    }

    public static void Clear(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;
        if (Store.flags.Remove(flag)) Save();
    }

    public static bool IsChoiceUsed(string choiceId)
    {
        if (string.IsNullOrEmpty(choiceId)) return false;
        return Store.usedChoices.Contains(choiceId);
    }

    public static void MarkChoiceUsed(string choiceId)
    {
        if (string.IsNullOrEmpty(choiceId)) return;
        if (!Store.usedChoices.Contains(choiceId)) { Store.usedChoices.Add(choiceId); Save(); }
    }

    public static void ResetAll()
    {
        data = new Data();
        Save();
    }
}