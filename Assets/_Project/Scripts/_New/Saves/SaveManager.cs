using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private string savePath;
    private ISaveable<MoneySaveData> moneySaveable;
    private ISaveable<ProductSaveData> productSaveable;
    private ISaveable<OrderSaveData> orderSaveable;

    // Получаем менеджеры как интерфейсы + подписываемся на события
    public void Initialize(
        ISaveable<MoneySaveData> money,
        ISaveable<ProductSaveData> product,
        ISaveable<OrderSaveData> order,
        MoneyManager mm,
        ProductManager pm,
        OrderManager om)
    {
        moneySaveable = money;
        productSaveable = product;
        orderSaveable = order;
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Debug.Log("Путь к сохранению: " + savePath);

        // Подписываемся на события для автоматического сохранения
        mm.OnMoneyChanged += OnChanged;
        pm.OnProductUnlocked += OnChanged;
        om.OnOrderGenerated += OnChanged;
    }

    public GameSaveData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        // Значения по умолчанию
        return new GameSaveData
        {
            moneyData = new MoneySaveData { money = 1000 },
            productData = new ProductSaveData { unlockedProductIds = new string[0] },
            orderData = new OrderSaveData { productIds = new string[0] }
        };
    }

    public void Save()
    {
        var data = new GameSaveData
        {
            moneyData = moneySaveable.GetSaveData(),
            productData = productSaveable.GetSaveData(),
            orderData = orderSaveable.GetSaveData()
        };
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(savePath, json);
    }

    // Общий обработчик событий
    private void OnChanged<T>(T arg) => Save();

    // Варианты для событий с параметрами
    private void OnMoneyChanged(int money) => Save();
    private void OnProductUnlocked(ProductData p) => Save();
    private void OnOrderGenerated(List<ProductData> orders) => Save();

    // Отписка (если нужно)
    public void Cleanup(MoneyManager mm, ProductManager pm, OrderManager om)
    {
        mm.OnMoneyChanged -= OnMoneyChanged;
        pm.OnProductUnlocked -= OnProductUnlocked;
        om.OnOrderGenerated -= OnOrderGenerated;
    }
}
[System.Serializable]
public class GameSaveData
{
    public MoneySaveData moneyData;
    public ProductSaveData productData;
    public OrderSaveData orderData;
}