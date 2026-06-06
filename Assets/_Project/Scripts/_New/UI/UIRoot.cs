using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private GameObject moneyUIPrefab;
    [SerializeField] private GameObject productListPrefab; // НОВОЕ
    
    private MoneyUI moneyUI;
    private ProductListUI productListUI; // НОВОЕ

    public void Initialize(GameServices services)
    {
        // Создаём окно баланса
        var moneyObj = Instantiate(moneyUIPrefab, transform);
        moneyUI = moneyObj.GetComponent<MoneyUI>();
        moneyUI.Init(services.moneyManager, windowManager);
        windowManager.RegisterWindow(moneyUI);
        windowManager.OpenWindow(moneyUI);

        // НОВОЕ: Создаём окно списка продуктов
        var listObj = Instantiate(productListPrefab, transform);
        productListUI = listObj.GetComponent<ProductListUI>();
        productListUI.Init(
            services.productManager, 
            services.moneyManager, 
            services.cartManager,
            services.shopManager, // Передаём ShopManager
            windowManager
        );
        windowManager.RegisterWindow(productListUI);
        // НЕ вызываем OpenWindow, так как у нас Вариант Б (открытие по кнопке)
    }
    
    // Публичный метод для открытия магазина (для кнопки на сцене)
    public void OpenShop()
    {
        if (productListUI != null)
            windowManager.OpenWindow(productListUI);
    }
}