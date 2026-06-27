using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class priceControl : MonoBehaviour
{
    public List<GameObject> productPrefabs = new List<GameObject>();
    public Dictionary<string, int> prices = new Dictionary<string, int>();
    public GameObject priceMenu;
    public ScrollRect productsContent;
    public GameObject productInfoPrefab;
    int menuOpenClosed;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            menuOpenClosed = 1 - menuOpenClosed;
            MenuLogic();
        }
    }
    void UpdateList()
    {
        foreach (Transform child in productsContent.content)
            Destroy(child.gameObject);
        foreach (var product in productPrefabs)
        {
            GameObject productInfo = Instantiate(productInfoPrefab, productsContent.content);
            var buttons = productInfo.GetComponentsInChildren<Button>();
            var texts = productInfo.GetComponentsInChildren<TMP_Text>();
            texts[0].text = product.GetComponent<Product>().Name;
            if (prices.ContainsKey(product.GetComponent<Product>().Name))
                texts[1].text = prices[product.GetComponent<Product>().Name].ToString();
            else
            {
                prices.Add(product.GetComponent<Product>().Name, product.GetComponent<Product>().sellPrice);
                UpdateList();
            }
            buttons[0].onClick.AddListener(() => SelectedProductPriceControl(true, product.GetComponent<Product>()));
            buttons[1].onClick.AddListener(() => SelectedProductPriceControl(false, product.GetComponent<Product>()));
        }
    }

    void MenuLogic()
    {
        if (menuOpenClosed == 1)
        {
            Cursor.lockState = CursorLockMode.None;
            priceMenu.SetActive(true);
            UpdateList();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            priceMenu.SetActive(false);
        }
    }
    void SelectedProductPriceControl(bool add, Product selectedProduct)
    {
        if (add)
        {
            if (prices.ContainsKey(selectedProduct.Name)) prices[selectedProduct.Name] = prices[selectedProduct.Name] += 1;
        }
        else
        {
            if (prices.ContainsKey(selectedProduct.Name)) prices[selectedProduct.Name] = prices[selectedProduct.Name] -= 1;
        }
        UpdateList();
    }
}
