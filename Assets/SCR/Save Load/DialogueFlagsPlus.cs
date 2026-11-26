using System.Collections.Generic;

public static class DialogueFlagsPlus
{
    private static readonly HashSet<string> _used = new();
    public static string MakeKey(DialogueData dialogue, int nodeIndex, int choiceIndex)
        => $"{dialogue.name}:{nodeIndex}:{choiceIndex}";

    public static void MarkUsed(string key) => _used.Add(key);
    public static bool IsUsed(string key) => _used.Contains(key);

    public static List<string> GetAll() => new List<string>(_used);

    public static void SetAll(List<string> list)
    {
        _used.Clear();
        if (list != null) foreach (var k in list) _used.Add(k);
    }

    public static void Clear() => _used.Clear();
}