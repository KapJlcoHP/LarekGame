using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Order : MonoBehaviour
{
    public WalletSystem playerWallet;
    public List<Product> products = new List<Product>();
    public TMP_Text orderText;
    public Dictionary<string, int> order = new Dictionary<string, int>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) GenerateOrder();
        UpdateOrderDisplay();
    }
    void GenerateOrder()
    {
        order.Clear();
        var uniqueProducts = products;

        if (uniqueProducts.Count > 0)
        {
            int itemsInOrder = Random.Range(1, uniqueProducts.Count + 1);
            for (int i = 0; i < itemsInOrder; i++)
            {
                Product randomProduct = uniqueProducts[Random.Range(0, uniqueProducts.Count)];
                if (order.ContainsKey(randomProduct.Name))
                    order[randomProduct.Name]++;
                else
                    order.Add(randomProduct.Name, 1);
            }
        }
    }
    public bool TryCompleteOrder(string productName)
    {
        if (order.ContainsKey(productName) && order[productName] > 0)
        {
            order[productName]--;
            if (order[productName] == 0)
                order.Remove(productName);
            return true;
        }
        return false;
    }

    void UpdateOrderDisplay()
    {
        orderText.text = "Order:\n";
        foreach (var item in order)
        {
            orderText.text += $"{item.Key} x{item.Value}\n";
        }
    }
}
