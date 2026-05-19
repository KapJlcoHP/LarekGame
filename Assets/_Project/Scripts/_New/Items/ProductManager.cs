using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProductManager
{
    public List<ProductData> AllProducts = new List<ProductData>();//public List<ProductData> AllProducts { get; private set; } = new List<ProductData>();
    private Dictionary<ProductData, bool> unlockedState = new Dictionary<ProductData, bool>();

    public event Action OnProductsLoaded;
    public event Action<ProductData> OnProductUnlocked;

    // Загрузка каталога и добавление предметов
    public Task AddCatalog(ProductDatabase catalog)
    {
        foreach (var product in catalog.allItems)
        {
            if (!AllProducts.Contains(product))
            {
                AllProducts.Add(product);
                unlockedState[product] = false; // начальное состояние
            }
        }
        OnProductsLoaded?.Invoke();
        return Task.CompletedTask;
    }

    public bool IsUnlocked(ProductData product) => unlockedState.TryGetValue(product, out var val) && val;

    public void Unlock(ProductData product)
    {
        unlockedState[product] = true;
        OnProductUnlocked?.Invoke(product);
    }
}