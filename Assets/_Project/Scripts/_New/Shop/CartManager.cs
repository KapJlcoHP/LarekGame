using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Управляет оптовой корзиной игрока.
/// Хранит выбранные продукты и их количество, позволяет оформить сделку (Deal),
/// списывая деньги через MoneyManager и уведомляя подписчиков о результате.
/// </summary>
public class CartManager
{
    private readonly Dictionary<ProductData, int> items = new();
    private readonly MoneyManager moneyManager;

    /// <summary>Событие при любом изменении содержимого корзины (добавление, удаление, очистка).</summary>
    public event System.Action OnCartChanged;

    /// <summary>Событие после успешной сделки, передаёт словарь купленных продуктов и их количество.</summary>
    public event System.Action<Dictionary<ProductData, int>> OnDealPlaced;

    public bool IsEmpty => items.Count == 0;
    public IReadOnlyDictionary<ProductData, int> Items => items;

    public CartManager(MoneyManager moneyManager)
    {
        this.moneyManager = moneyManager;
    }

    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var qty in items.Values)
            total += qty;
        return total;
    }

    public bool CanAffordDeal()
    {
        return !IsEmpty && moneyManager.Balance() >= GetTotalCost();
    }

    /// <summary>
    /// Добавляет продукт в корзину (по умолчанию 1 штуку).
    /// </summary>
    public void AddToCart(ProductData product, int quantity = 1)
    {
        if (product == null) return;

        items.TryGetValue(product, out int current);
        items[product] = current + quantity;

        OnCartChanged?.Invoke();
    }

    /// <summary>
    /// Убирает указанное количество продукта из корзины.
    /// Если количество становится <= 0, продукт удаляется из корзины.
    /// </summary>
    public void RemoveFromCart(ProductData product, int quantity = 1)
    {
        if (product == null || !items.ContainsKey(product)) return;

        items[product] -= quantity;
        if (items[product] <= 0)
            items.Remove(product);

        OnCartChanged?.Invoke();
    }

    /// <summary>Полностью очищает корзину.</summary>
    public void ClearCart()
    {
        items.Clear();
        OnCartChanged?.Invoke();
    }

    /// <summary>Возвращает текущее количество продукта в корзине.</summary>
    public int GetQuantity(ProductData product)
    {
        return items.TryGetValue(product, out int qty) ? qty : 0;
    }

    /// <summary>Общая стоимость всех товаров в корзине.</summary>
    public int GetTotalCost()
    {
        return items.Sum(kvp => kvp.Key.buyPrice * kvp.Value);
    }

    /// <summary>
    /// Оформляет сделку: проверяет баланс, списывает деньги, очищает корзину и вызывает событие OnDealPlaced.
    /// </summary>
    /// <returns>true, если сделка успешна; false, если корзина пуста или недостаточно денег.</returns>
    public bool PlaceDeal()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("Корзина пуста");
            return false;
        }

        int totalCost = GetTotalCost();
        if (moneyManager.Balance() < totalCost)
        {
            Debug.LogWarning($"Недостаточно денег для сделки (нужно {totalCost}, есть {moneyManager.Balance()})");
            return false;
        }

        bool success = moneyManager.SpendMoney(totalCost);
        if (!success)
            return false;

        // Передаём копию словаря, потому что сразу после ClearCart() оригинал опустеет
        var purchasedItems = new Dictionary<ProductData, int>(items);
        ClearCart(); // вызовет OnCartChanged

        OnDealPlaced?.Invoke(purchasedItems);
        return true;
    }
}