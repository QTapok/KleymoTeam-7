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

    public CraftRecipe FindRecipe(ItemData a, ItemData b)
    {
        if (a == null || b == null) return null;

        foreach (var r in recipes)
        {
            // Проверяем совпадение предметов
            if ((r.inputA == a && r.inputB == b) ||
                (r.inputA == b && r.inputB == a))
            {
                // И проверяем, выучен ли этот рецепт
                if (DialogueFlags.HasRecipe(r.recipeId))
                {
                    return r;
                }
                else
                {
                    // Если предметы подходят, но рецепт не выучен
                    DebugLogger.Log($"[DB] Игрок еще не знает рецепт для: {a.title} + {b.title}");
                    return null;
                }
            }
        }
        return null;
    }
}