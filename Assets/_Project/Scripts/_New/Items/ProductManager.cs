using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Android.Gradle;
using UnityEngine;
using static ProductManager;

public class ProductManager : ISaveable<ProductSaveData>
{
    public List<ProductData> AllProducts = new List<ProductData>();//public List<ProductData> AllProducts { get; private set; } = new List<ProductData>();
    private Dictionary<ProductData, bool> unlockedState = new Dictionary<ProductData, bool>();

    public event Action OnProductsLoaded;
    public event Action<ProductData> OnProductUnlocked;

    // «агрузка каталога и добавление предметов
    public Task AddCatalog(ProductDatabase catalog)
    {
        foreach (var product in catalog.allItems)
        {
            if (!AllProducts.Contains(product))
            {
                AllProducts.Add(product);
                unlockedState[product] = false; // начальное состо€ние
            }
        }
        OnProductsLoaded?.Invoke();
        return Task.CompletedTask;
    }

    public bool IsUnlocked(ProductData product) => unlockedState.TryGetValue(product, out var val) && val;
    public ProductSaveData GetSaveData()
    {
        var unlocked = new List<string>();
        foreach (var p in AllProducts)
            if (IsUnlocked(p)) unlocked.Add(p.itemName);
        return new ProductSaveData { unlockedProductIds = unlocked.ToArray() };
    }

    public void LoadSaveData(ProductSaveData data)
    {
        foreach (var id in data.unlockedProductIds)
        {
            var product = AllProducts.Find(p => p.itemName == id);
            if (product != null)
                unlockedState[product] = true;  // тихое восстановление
        }
    }
    public void Unlock(ProductData product)
    {
        unlockedState[product] = true;
        OnProductUnlocked?.Invoke(product);
    } 
}
    [System.Serializable]
    public class ProductSaveData
    {
        public string[] unlockedProductIds;
    }
