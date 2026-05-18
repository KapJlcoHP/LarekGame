using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//комментарий для коммита
public class SellTriggerHandler : MonoBehaviour
{
    public Order orderSystem;
    public WalletSystem walletSystem;
    

    void TrySellProducts(Product product)
    {
        if (orderSystem.order[product.Name] > 1)
        {
            orderSystem.order[product.Name]--;
            walletSystem.AddMoney(product.sellPrice);
            orderSystem.UpdateOrderDisplay();
            Destroy(product.gameObject);
        }
        else {
            walletSystem.AddMoney(product.sellPrice);
            orderSystem.order[product.Name]--;
            orderSystem.order.Remove(product.Name);
            orderSystem.UpdateOrderDisplay();
            Destroy(product.gameObject);
        }
        if (orderSystem.TryCompleteOrder()) { orderSystem.GenerateOrder(); }
    }
    void OnTriggerEnter(Collider other)
    {
        Product product = other.GetComponent<Product>();

        if (product != null && orderSystem.order.ContainsKey(product.Name))
        {
            TrySellProducts(other.GetComponent<Product>());
        }
    }
}
