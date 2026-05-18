using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(fileName = "NewItem", menuName = "Larek Items/Create item data")]
public class ProductData : ScriptableObject
{
    public string itemName;
    public AssetReferenceSprite icon;
    public int buyPrice;
    public int sellPrice;
    public int unlockPrice;
    public AssetReferenceGameObject prefab;
}
