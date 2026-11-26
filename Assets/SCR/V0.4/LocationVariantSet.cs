using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Location Variant Set", fileName = "LocationVariantSet")]
public class LocationVariantSet : ScriptableObject
{
    [Tooltip("ID локации")]
    public string locationId;

    [Tooltip("Все варианты префабов этой локации")]
    public List<GameObject> variants = new List<GameObject>();

    [Tooltip("Можно ли повторить вчерашний вариант")]
    public bool allowRepeatIfOnlyOne = true;
}