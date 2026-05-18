using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    private int money = 0;
    public Text moneyText;
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Added {amount} money. Total: {money}");
        UpdateMoneyUI();
    }
    public void SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log($"Spent {amount} money. Total: {money}");
            UpdateMoneyUI();
        }
        else
        {
            Debug.LogWarning("Not enough money to spend!");
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
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }
    

}
