using UnityEngine;

public class Product : MonoBehaviour
{
    [Header("Options")]
    public string Name;
    public int sellPrice;
    public int buyPrice;
    public int defaultSellPrice;
    public int defaultBuyPrice;

    void FixedUpdate()
    {
        priceControl priceController = FindAnyObjectByType<priceControl>();
        if (priceController.prices.ContainsKey(Name))
            sellPrice = priceController.prices[Name];
    }

    public void ResetParams()
    {
        sellPrice = defaultSellPrice;
        buyPrice = defaultBuyPrice;
    }
}
