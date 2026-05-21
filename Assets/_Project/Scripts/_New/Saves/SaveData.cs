[System.Serializable]
public class SaveData
{
    public int money;
    public string[] unlockedProductIds;   // идентификаторы разблокированных продуктов
    public OrderData currentOrder;        // состояние текущего заказа (если есть)
}

[System.Serializable]
public class OrderData
{
    public string productId;   // идентификатор продукта в заказе
    public int reward;
}
