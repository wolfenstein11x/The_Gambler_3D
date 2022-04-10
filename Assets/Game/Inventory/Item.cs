using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnnamedItem", menuName = "Inventory Item")]
public class Item : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string itemDescription;
}