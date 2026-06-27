using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Спавнит физические префабы товаров в точке разгрузки после оформления сделки.
/// </summary>
public class DealSpawner
{
    private readonly CartManager cartManager;
    private readonly Vector3 spawnPosition;
    private readonly Quaternion spawnRotation;

    public DealSpawner(CartManager cartManager, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        this.cartManager = cartManager;
        this.spawnPosition = spawnPosition;
        this.spawnRotation = spawnRotation;
        cartManager.OnDealPlaced += OnDealPlaced;
    }

    private void OnDealPlaced(Dictionary<ProductData, int> purchasedItems)
    {
        _ = SpawnItemsAsync(purchasedItems);
    }

    private async Task SpawnItemsAsync(Dictionary<ProductData, int> purchasedItems)
    {
        int spawnIndex = 0;

        foreach (var kvp in purchasedItems)
        {
            ProductData product = kvp.Key;
            int quantity = kvp.Value;

            if (product.prefab == null || !product.prefab.RuntimeKeyIsValid())
            {
                Debug.LogWarning($"У товара {product.itemName} нет префаба для спавна");
                continue;
            }

            for (int i = 0; i < quantity; i++)
            {
                Vector3 offset = new Vector3(spawnIndex * 0.5f, 0f, 0f);
                Vector3 position = spawnPosition + offset;

                AsyncOperationHandle<GameObject> handle =
                    Addressables.InstantiateAsync(product.prefab, position, spawnRotation);

                await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogError($"Не удалось заспавнить {product.itemName}");

                spawnIndex++;
            }
        }
    }

    public void Cleanup()
    {
        cartManager.OnDealPlaced -= OnDealPlaced;
    }
}
