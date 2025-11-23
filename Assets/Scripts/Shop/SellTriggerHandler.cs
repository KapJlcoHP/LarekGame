using System.Collections.Generic;
using UnityEngine;
//комментарий для коммита
public class SellTriggerHandler : MonoBehaviour
{
    public Order orderSystem;
    public WalletSystem walletSystem;
    public List<Product> productsInTrigger = new List<Product>();

    void TrySellProducts()
    {
        for (int i = productsInTrigger.Count - 1; i >= 0; i--)
        {
            Product product = productsInTrigger[i];
            if (orderSystem.TryCompleteOrder(product.Name))
            {
                walletSystem.AddMoney(product.sellPrice);
                productsInTrigger.RemoveAt(i);
                Destroy(product.gameObject);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Product product = other.GetComponent<Product>();
        if (product != null && !productsInTrigger.Contains(product))
        {
            productsInTrigger.Add(product);
            TrySellProducts();
        }
    }
    void OnTriggerExit(Collider other)
    {
        Product product = other.GetComponent<Product>();
        if (product != null)
        {
            productsInTrigger.Remove(product);
        }
    }
}
