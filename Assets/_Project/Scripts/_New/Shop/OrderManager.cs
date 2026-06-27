using NUnit.Framework;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Collections.Generic;

public class OrderManager : ISaveable<OrderSaveData>
{
    public event System.Action<List<ProductData>> OnOrderGenerated;
    public IReadOnlyList<ProductData> CurrentOrders => currentOrders;
    private MoneyManager moneyManager;
    private ProductManager productManager;
    private List<ProductData> currentOrders = new List<ProductData>();
    private int maxProductsInOrder = 3; // Максимальное количество продуктов в одном заказе
    public void SetMoneyManager(MoneyManager moneyManager)
    {
        this.moneyManager = moneyManager;
    }
    public void SetProductManager(ProductManager productManager)
    {
        this.productManager = productManager;
    }

    public void GenerateOrder()
    {
        currentOrders.Clear();

        // Только неразблокированные и доступные по деньгам
        var candidates = productManager.AllProducts
            .FindAll(p => productManager.IsUnlocked(p) && p.buyPrice <= moneyManager.Balance());

        if (candidates.Count == 0)
        {
            Debug.LogWarning("Нет доступных продуктов для заказа");
            OnOrderGenerated?.Invoke(currentOrders);
            return;
        }

        // Выбираем случайные без повторов
        int count = Mathf.Min(maxProductsInOrder, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, candidates.Count);
            currentOrders.Add(candidates[index]);
            candidates.RemoveAt(index);
        }
        Debug.Log($"GenerateOrder: сгенерирован заказ из {currentOrders.Count} продуктов");
        OnOrderGenerated?.Invoke(currentOrders);
    }
    /*public void RestoreOrder(List<string> productIds)
    {
        currentOrders.Clear();
        foreach (string id in productIds)
        {
            var product = productManager.AllProducts.Find(p => p.itemName == id);
            if (product != null)
                currentOrders.Add(product);
            else
                Debug.LogWarning($"Продукт с id '{id}' не найден в каталоге");
        }
        // Событие НЕ вызываем — это восстановление, не новое изменение
        Debug.Log($"Заказ восстановлен: {currentOrders.Count} продуктов");
    }*/
    public OrderSaveData GetSaveData()
    {
        var ids = new string[currentOrders.Count];
        for (int i = 0; i < currentOrders.Count; i++)
            ids[i] = currentOrders[i].itemName;
        return new OrderSaveData { productIds = ids };
    }

    public void LoadSaveData(OrderSaveData data)
    {

        if (data.productIds.Length == 0)
        {
            GenerateOrder();
            return;
        }

        // Проверяем, не разблокированы ли уже продукты из сохранённого заказа
        var products = new List<ProductData>();
        foreach (var id in data.productIds)
        {
            var product = productManager.AllProducts.Find(p => p.itemName == id);
            if (product != null && productManager.IsUnlocked(product))
                products.Add(product);
        }
        Debug.Log($"LoadSaveData: восстанавливаю {products.Count} продуктов");
        if (products.Count == 0)
        {
            // Все продукты из заказа уже разблокированы — генерируем новый
            GenerateOrder();
        }
        else
        {
            currentOrders = products;
            Debug.Log($"Заказ восстановлен: {currentOrders.Count} продуктов");
            // Не вызываем событие, чтобы не зациклить сохранение
        }
    }
}
[System.Serializable]
public class OrderSaveData
{
    public string[] productIds;   // идентификаторы продуктов в заказе
}
