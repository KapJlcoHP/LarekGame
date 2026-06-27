using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(fileName = "New item database", menuName = "Larek Items/Create items database")]
public class ProductDatabase : ScriptableObject
{
    public ProductData[] allItems;
}
