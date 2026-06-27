using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private string savePath;
    private ISaveable<MoneySaveData> moneySaveable;
    private ISaveable<ProductSaveData> productSaveable;
    private ISaveable<OrderSaveData> orderSaveable;

    // �������� ��������� ��� ���������� + ������������� �� �������
    public void Initialize(
        ISaveable<MoneySaveData> money,
        ISaveable<ProductSaveData> product,
        ISaveable<OrderSaveData> order,
        MoneyManager mm,
        ProductManager pm,
        OrderManager om)
    {
        // Добавляем проверки на null
        if (mm == null) throw new System.ArgumentNullException(nameof(mm), "MoneyManager равен null!");
        if (pm == null) throw new System.ArgumentNullException(nameof(pm), "ProductManager равен null!");
        if (om == null) throw new System.ArgumentNullException(nameof(om), "OrderManager равен null!");

        moneySaveable = money;
        productSaveable = product;
        orderSaveable = order;
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Debug.Log("Путь для сохранения: " + savePath);

        // Подписываемся на события для автосохранения при изменениях
        mm.OnMoneyChanged += OnMoneyChanged;
        pm.OnProductUnlocked += OnProductUnlocked;
        om.OnOrderGenerated += OnOrderGenerated;
    }

    public GameSaveData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        // �������� �� ���������
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

    private void OnMoneyChanged(int money) => Save();
    private void OnProductUnlocked(ProductData p) => Save();
    private void OnOrderGenerated(List<ProductData> orders) => Save();

    // ������� (���� �����)
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