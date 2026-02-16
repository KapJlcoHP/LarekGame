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
        string maxProductToBuy = null;
        var uniqueProducts = products;

        if (uniqueProducts.Count > 0)
        {
            int itemsInOrder = Random.Range(1, 20);
            
            foreach(Product product in products)
            {
                if(playerWallet.wallet >= product.buyPrice)
                {
                    maxProductToBuy = product.Name;
                }
            }
            for (int i = 0; i < itemsInOrder; i++)
            {
                Product randomProduct = uniqueProducts[Random.Range(0, uniqueProducts.Count)];
                if (randomProduct.isUnlocked)
                {
                    if (order.ContainsKey(randomProduct.Name) && ValuesSum() + randomProduct.buyPrice < playerWallet.wallet)
                    {
                        order[randomProduct.Name]++;
                    }

                    else if (randomProduct.buyPrice + ValuesSum() < playerWallet.wallet)
                    {
                        order.Add(randomProduct.Name, 1);
                    }     
                }
            }
           
        }
        if (order.Count == 0 && maxProductToBuy != null)
        {

            order.Add(maxProductToBuy, 1);
        }
        UpdateOrderDisplay();
    }
    public bool TryCompleteOrder()
    {
        if (order.Count == 0)
        {
            return true;
        }
        return false;
    }

    public void UpdateOrderDisplay()
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
