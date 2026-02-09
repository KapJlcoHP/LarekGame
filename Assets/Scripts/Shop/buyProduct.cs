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
    public Button unLockButton;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            menuOpenClosed = 1 - menuOpenClosed;
            MenuLogic();
        }
    }
    public void MenuLogic()
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
                Image image = productInfo.transform.GetChild(1).GetComponent<Image>();
                GameObject state = productInfo.transform.GetChild(2).gameObject;
                var text = state.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                var unblockButton = state.transform.GetChild(1).gameObject.GetComponent<Button>();
                
                

                foreach (var im in productSprites)
                {
                    if (im.name == product.GetComponent<Product>().Name)
                    {
                        if (product.GetComponent<Product>().isUnlocked == true)
                        {
                            image.sprite = im;
                            state.SetActive(false);
                            break;
                        }
                        else
                        {
                            image.sprite = blockedImage;

                            state.SetActive(true);
                            text.text = $"{product.GetComponent<Product>().unlockPrice}";
                            break;

                        }
                    }
                }
                var button = productInfo.GetComponent<Button>();
                unblockButton.onClick.AddListener(() => OnUnlock(product.GetComponent<Product>(), state, image, button));
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
    public void SelectProduct(Product productData)
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
    public void OnUnlock(Product product, GameObject sp, Image image,Button buyButton)
    {
        print("OnUnlock");
        if (walletSystem.wallet >= product.unlockPrice)
        {
            walletSystem.RemoveMoney(product.unlockPrice);

            product.isUnlocked = true;
            buyButton.interactable = true;
            sp.SetActive(false);
            foreach (var sprite in productSprites)
            {
                if (product.Name == sprite.name)
                {
                    image.sprite = sprite;
                    break;
                }
            }
        }
    }
}
