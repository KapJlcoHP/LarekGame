[System.Serializable]
public class SaveData
{
    public int money;
    public string[] unlockedProductIds;   // идентификаторы разблокированных продуктов (itemName)
    public string[] orderProductIds;      // идентификаторы продуктов в текущем заказе
}