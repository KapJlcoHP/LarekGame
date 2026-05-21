using NUnit.Framework;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Collections.Generic;

public class OrderManager
{
    public event System.Action<List<ProductData>> OnOrderGenerated;
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
        int productsCount = maxProductsInOrder;
        // Логика генерации заказа, например, случайный выбор продукта из productManager.AllProducts
        if (productManager.AllProducts.Count > 0)
        {
            for (int i = 0; i < productsCount; i++)
            { 
                var randomProduct = productManager.AllProducts[Random.Range(0, productManager.AllProducts.Count)];
                if (productManager.IsUnlocked(randomProduct)&&randomProduct.buyPrice <= moneyManager.Balance())
                    Debug.Log($"Generated order for: {randomProduct.name}");
                currentOrders.Add(randomProduct);
            }
            // Здесь мо��но добавить логику для отображения заказа игроку и обработки его выполнения
        }
        else
        {
            Debug.LogWarning("No products available to generate an order.");
        }
        OnOrderGenerated?.Invoke(currentOrders);
    }
    public void RestoreOrder(List<string> productIds)
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
    }

}
