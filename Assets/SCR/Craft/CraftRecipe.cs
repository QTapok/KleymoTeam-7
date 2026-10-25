using UnityEngine;

[CreateAssetMenu(fileName = "CraftRecipe", menuName = "Scriptable Objects/CraftRecipe")]
public class CraftRecipe : ScriptableObject
{
    public ItemData inputA;
    public ItemData inputB;
    public ItemData result;
}