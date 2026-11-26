using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;

    public string title;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("Если это записка — какие рецепты она открывает")]
    public CraftRecipe[] unlockRecipes;
}