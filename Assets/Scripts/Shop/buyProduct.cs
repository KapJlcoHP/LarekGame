using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buyProduct : MonoBehaviour
{
    public List<Sprite> productSprites = new List<Sprite>();
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
    public Sprite blockedImage;


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
                var image = productInfoPrefab.transform.GetChild(1).GetComponent<Image>();
                var state = productInfoPrefab.transform.GetChild(2).gameObject;
                var text = state.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                foreach (var im in productSprites)
                {
                    if (im.name == product.GetComponent<Product>().Name)
                    {
                        if (product.GetComponent<Product>().isUnlocked == true)
                        {
                            image.sprite = im;
                            print("1 " + product.GetComponent<Product>().Name);
                            state.SetActive(false);
                            break;
                        }
                        else
                        {
                            image.sprite = blockedImage;
                            print("2 " + product.GetComponent<Product>().Name);

                            state.SetActive(true);
                            text.text = $"{product.GetComponent<Product>().UnlockPrice}";
                            break;

                        }
                    }
                }
                GameObject productInfo = Instantiate(productInfoPrefab, productsContent.content);

                var button = productInfo.GetComponent<Button>();
                if (product.GetComponent<Product>().isUnlocked)
                {
                    button.interactable = true;
                }
                else
                {
                    button.interactable = false;
                }
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
