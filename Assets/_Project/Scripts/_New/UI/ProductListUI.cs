using UnityEngine;

public class ProductListUI : UIWindow
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject productButtonPrefab;
    [SerializeField] private CartUI cartUI;

    private ProductManager productManager;
    private MoneyManager moneyManager;
    private CartManager cartManager;
    private ShopManager shopManager;

    public void Init(
        ProductManager pm,
        MoneyManager mm,
        CartManager cm,
        ShopManager sm,
        WindowManager wm)
    {
        base.Init(wm);
        productManager = pm;
        moneyManager = mm;
        cartManager = cm;
        shopManager = sm;

        if (cartUI == null)
            cartUI = GetComponentInChildren<CartUI>(true);

        cartUI?.Init(cm, mm);

        productManager.OnProductsLoaded += BuildProductList;

        if (productManager.AllProducts.Count > 0)
            BuildProductList();
    }

    public override void Show()
    {
        base.Show();
        cartUI?.Refresh();
    }

    private void BuildProductList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var product in productManager.AllProducts)
        {
            var buttonObj = Instantiate(productButtonPrefab, contentParent);
            var buttonUI = buttonObj.GetComponent<ProductButtonUI>();
            buttonUI.Setup(product, productManager, moneyManager, cartManager, shopManager);
        }
    }

    protected override void OnDestroy()
    {
        if (productManager != null)
            productManager.OnProductsLoaded -= BuildProductList;
        base.OnDestroy();
    }
}
