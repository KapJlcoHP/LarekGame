using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Order : MonoBehaviour
{
    public WalletSystem playerWallet;
    public Dictionary<string, Product> productsDict = new Dictionary<string,Product>();
    public TMP_Text orderText;
    public Dictionary<string, int> order = new Dictionary<string, int>();
    public buyProduct buyProducts;
    public List<Product> products = new List<Product>();
    public int debug;
    public int orderSum = 0;
   public void Start()
    {
        UpdateProducts();
        GenerateOrder();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) GenerateOrder();
        
    }
    public void GenerateOrder()
    {
        UpdateProducts();
        Debug.Log("Генерируется заказ!");
        order.Clear();
        var uniqueProducts = products;

        if (uniqueProducts.Count > 0)
        {
            int itemsInOrder = Random.Range(1, 20);
            for (int i = 0; i < itemsInOrder; i++)
            {
                Product randomProduct = uniqueProducts[Random.Range(0, uniqueProducts.Count)];
                if (randomProduct.isUnlocked)
                {
                    if (order.ContainsKey(randomProduct.Name) && ValuesSum() + randomProduct.buyPrice < playerWallet.wallet)
                    {
                        order[randomProduct.Name]++;
                    }

                    else if (randomProduct.buyPrice < playerWallet.wallet)
                    {
                        order.Add(randomProduct.Name, 1);
                    }
                    
                }
            }
        }
        
        UpdateOrderDisplay();
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

    private void UpdateOrderDisplay()
    {
        Debug.Log("Обновляется текст!");
        orderText.text = "Order:\n";
        foreach (var item in order)
        {
            orderText.text += $"{item.Key} x{item.Value}\n";
        }
    }
    public void UpdateProducts()
    {
        foreach(GameObject prefab in buyProducts.productPrefabs)
        {
            var product = prefab.GetComponent<Product>();
            if(product != null)
            {
                if (product.isUnlocked && !productsDict.ContainsKey(product.Name))
                {
                    productsDict.Add(product.Name, product);
                    products.Add(product);
                }
            }
        }
    }
    private int ValuesSum()
    {
        int sum = 0;
        foreach(var product in order.Keys)
        {
            sum += order[product] * productsDict[product].buyPrice;
        }
        return sum;
    }
}
