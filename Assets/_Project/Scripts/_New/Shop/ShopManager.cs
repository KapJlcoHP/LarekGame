using UnityEngine;

public class ShopManager
{
    private readonly ProductManager productManager;
    private readonly MoneyManager moneyManager;

    public ShopManager(ProductManager productManager, MoneyManager moneyManager)
    {
        this.productManager = productManager;
        this.moneyManager = moneyManager;
    }

    public bool TryUnlockProduct(ProductData product)
    {
        if (product == null || productManager.IsUnlocked(product)) return false;

        if (moneyManager.SpendMoney(product.unlockPrice))
        {
            productManager.Unlock(product); // ProductManager сам кинет событие OnProductUnlocked
            return true;
        }

        return false;
    }

        public bool CanAffordUnlock(ProductData product)
        {
            if (product == null || productManager.IsUnlocked(product)) return false;
            return moneyManager.Balance() >= product.unlockPrice;
        }
    }