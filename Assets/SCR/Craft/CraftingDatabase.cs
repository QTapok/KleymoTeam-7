using System.Collections.Generic;
using UnityEngine;

public class CraftingDatabase : MonoBehaviour
{
    public static CraftingDatabase Instance;

    [SerializeField] private List<CraftRecipe> recipes = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    //Ищет рецепт
    public CraftRecipe FindRecipe(ItemData a, ItemData b)
    {
        if (a == null || b == null) return null;

        DebugLogger.Log($"[DB] Ищу рецепт для: {a.title} + {b.title}");

        foreach (var r in recipes)
        {
            if ((r.inputA == a && r.inputB == b) ||
                (r.inputA == b && r.inputB == a))
                return r;
        }
        return null;
    }
}