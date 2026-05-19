using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ScenePrefab", menuName = "Larek items/Create scene prefab")]
public class ScenePrefab : ScriptableObject
{
    public AssetReferenceGameObject scenePref;
    public AssetReferenceGameObject playerPref;
    public Vector3 playerPos;
}
