using UnityEngine;

[CreateAssetMenu(fileName = "CraftRecipe", menuName = "Scriptable Objects/CraftRecipe")]
public class CraftRecipe : ScriptableObject
{
    [Tooltip("Уникальный ID рецепта (например: recipe_knife)")]
    public string recipeId;

    public ItemData inputA;
    public ItemData inputB;
    public ItemData result;
}