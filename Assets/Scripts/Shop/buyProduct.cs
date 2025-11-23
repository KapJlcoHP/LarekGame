using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buyProduct : MonoBehaviour
{
    public List<GameObject> productPrefabs = new List<GameObject>();
    public WalletSystem walletSystem;
    public GameObject buyMenu;
    public ScrollRect productsContent;
    public GameObject productInfoPrefab;
    public Product selectedProduct;
    public TMP_Text selectedProductName;
    public TMP_Text selectedProductPrice;
    public Transform productSpawn;
    int menuOpenClosed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            menuOpenClosed = 1 - menuOpenClosed;
            MenuLogic();
        }
    }
    void MenuLogic()
    {
        if (menuOpenClosed == 1)
        {
            Cursor.lockState = CursorLockMode.None;
            buyMenu.SetActive(true);
            foreach (Transform child in productsContent.content)
                Destroy(child.gameObject);
            foreach (var product in productPrefabs)
            {
                GameObject productInfo = Instantiate(productInfoPrefab, productsContent.content);
                var button = productInfo.GetComponent<Button>();
                var texts = productInfo.GetComponentsInChildren<TMP_Text>();
                button.onClick.AddListener(() => SelectProduct(product.GetComponent<Product>()));
                texts[0].text = product.GetComponent<Product>().Name;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            buyMenu.SetActive(false);
        }
    }
    void SelectProduct(Product productData)
    {
        selectedProduct = productData;
        selectedProductName.text = productData.Name;
        selectedProductPrice.text = productData.buyPrice.ToString();
    }
    public void BuyProduct()
    {
        if (selectedProduct)
        {
            if (walletSystem.wallet >= selectedProduct.buyPrice)
            {
                Instantiate(selectedProduct.gameObject, productSpawn.position, productSpawn.rotation);
                walletSystem.RemoveMoney(selectedProduct.buyPrice);
            }
        }
    }
}
