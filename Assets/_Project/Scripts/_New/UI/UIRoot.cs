using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private GameObject moneyUIPrefab;
    [SerializeField] private GameObject productListPrefab;

    private MoneyUI moneyUI;
    private ProductListUI productListUI;

    public void Initialize(GameServices services)
    {
        Transform uiParent = windowManager.transform;

        var moneyObj = Instantiate(moneyUIPrefab, uiParent);
        moneyUI = moneyObj.GetComponent<MoneyUI>();
        moneyUI.Init(services.moneyManager, windowManager);
        windowManager.RegisterWindow(moneyUI);
        windowManager.OpenWindow(moneyUI);

        var listObj = Instantiate(productListPrefab, uiParent);
        productListUI = listObj.GetComponent<ProductListUI>();
        productListUI.Init(
            services.productManager,
            services.moneyManager,
            services.cartManager,
            services.shopManager,
            windowManager
        );
        windowManager.RegisterWindow(productListUI);
    }

    public void OpenShop()
    {
        if (productListUI != null)
            windowManager.OpenWindow(productListUI);
    }
}
