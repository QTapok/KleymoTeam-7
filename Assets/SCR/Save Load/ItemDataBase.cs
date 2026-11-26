using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Database", fileName = "ItemDatabase")]
public class ItemDataBase : ScriptableObject
{
    public List<ItemData> items = new();

    private Dictionary<int, ItemData> _byId;

    public ItemData Get(int id)
    {
        if (_byId == null)
        {
            _byId = new Dictionary<int, ItemData>();
            foreach (var it in items)
                if (it != null) _byId[it.id] = it;
        }
        return _byId.TryGetValue(id, out var v) ? v : null;
    }

    public void Rebuild()
    {
        _byId = null;
    }
}