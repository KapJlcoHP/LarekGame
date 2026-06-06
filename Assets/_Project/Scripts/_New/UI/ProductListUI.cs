using UnityEngine;
public class ProductListUI : UIWindow
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject productButtonPrefab;

    private ProductManager productManager;
    private MoneyManager moneyManager;
    private CartManager cartManager;
    private ShopManager shopManager; // НОВОЕ

    public void Init(
        ProductManager pm, 
        MoneyManager mm, 
        CartManager cm, 
        ShopManager sm, // НОВОЕ
        WindowManager wm)
    {
        base.Init(wm);
        productManager = pm;
        moneyManager = mm;
        cartManager = cm;
        shopManager = sm; // Сохраняем ссылку

        // Подписываемся на загрузку каталога
        productManager.OnProductsLoaded += BuildProductList;
        
        // Если каталог уже загружен (мало ли), строим сразу
        if (productManager.AllProducts.Count > 0)
            BuildProductList();
    }

    private void BuildProductList()
    {
        // Очищаем контейнер
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Создаём кнопки для каждого продукта
        foreach (var product in productManager.AllProducts)
        {
            var buttonObj = Instantiate(productButtonPrefab, contentParent);
            var buttonUI = buttonObj.GetComponent<ProductButtonUI>();
            
            // Передаём ShopManager в карточку
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