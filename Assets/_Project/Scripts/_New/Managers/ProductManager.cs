using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public List<ProductData> AllProducts = new List<ProductData>();//public List<ProductData> AllProducts { get; private set; } = new List<ProductData>();
    private Dictionary<ProductData, bool> unlockedState = new Dictionary<ProductData, bool>();

    public event Action OnProductsLoaded;
    public event Action<ProductData> OnProductUnlocked;

    // Загрузка каталога и добавление предметов
    public void AddCatalog(ProductDatabase catalog)
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
    }

    public bool IsUnlocked(ProductData product) => unlockedState.TryGetValue(product, out var val) && val;

    public void Unlock(ProductData product)
    {
        unlockedState[product] = true;
        OnProductUnlocked?.Invoke(product);
    }
}