using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
public class MoneyManager : ISaveable<MoneySaveData>
{
    private int money = 0;
    public Text moneyText;
    public event Action<int> OnMoneyChanged;
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Added {amount} money. Total: {money}");
        UpdateMoneyUI();
    }
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log($"Spent {amount} money. Total: {money}");
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough money to spend!");
            return false;
        }
    }
    public void SetMoney(int amount)
    {
        money = amount;
        Debug.Log($"Money set to {money}");
        UpdateMoneyUI();
    }
    public void UpdateMoneyUI()
    {
        OnMoneyChanged?.Invoke(money);
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }
    public int Balance()
    {
        return money;
    }
    public MoneySaveData GetSaveData()
    {
        return new MoneySaveData { money = money };
    }

    public void LoadSaveData(MoneySaveData data)
    {
        money = data.money;
        UpdateMoneyUI();
    }
}
[System.Serializable]
public class MoneySaveData
{
    public int money;
}