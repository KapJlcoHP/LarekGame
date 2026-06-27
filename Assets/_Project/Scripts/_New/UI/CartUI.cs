using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Панель корзины внутри BuyMenu. Не отдельное окно — видимость управляет ProductListUI.
/// </summary>
public class CartUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject lineItemTemplate;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private Button dealButton;
    [SerializeField] private TextMeshProUGUI cartBadgeText;
    [SerializeField] private GameObject cartBadge;

    private CartManager cartManager;
    private MoneyManager moneyManager;
    private readonly List<CartLineItemUI> activeLines = new();

    public void Init(CartManager cart, MoneyManager money)
    {
        cartManager = cart;
        moneyManager = money;

        BindIfNeeded();

        if (lineItemTemplate != null)
            lineItemTemplate.SetActive(false);

        cartManager.OnCartChanged += Refresh;
        moneyManager.OnMoneyChanged += OnMoneyChanged;

        dealButton.onClick.RemoveListener(OnDealClicked);
        dealButton.onClick.AddListener(OnDealClicked);

        Refresh();
    }

    private void BindIfNeeded()
    {
        if (contentParent == null)
            contentParent = transform.Find("Mask/Content/ProductsList");

        if (lineItemTemplate == null && contentParent != null && contentParent.childCount > 0)
            lineItemTemplate = contentParent.GetChild(0).gameObject;

        if (totalCostText == null)
            totalCostText = transform.Find("DealSplit/TotalCost")?.GetComponent<TextMeshProUGUI>();

        if (dealButton == null)
            dealButton = transform.Find("DealSplit/DealButton")?.GetComponent<Button>();
    }

    public void Refresh()
    {
        RebuildList();
        UpdateTotal();
        UpdateDealButton();
        UpdateBadge();
    }

    private void RebuildList()
    {
        foreach (var line in activeLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        activeLines.Clear();

        if (lineItemTemplate == null || contentParent == null)
            return;

        foreach (var kvp in cartManager.Items)
        {
            var lineObj = Instantiate(lineItemTemplate, contentParent);
            lineObj.SetActive(true);
            var lineUI = lineObj.GetComponent<CartLineItemUI>();
            if (lineUI == null)
                lineUI = lineObj.AddComponent<CartLineItemUI>();
            lineUI.Setup(kvp.Key, cartManager);
            activeLines.Add(lineUI);
        }
    }

    private void UpdateTotal()
    {
        if (totalCostText != null)
            totalCostText.text = MoneyFormatter.Format(cartManager.GetTotalCost());
    }

    private void UpdateDealButton()
    {
        if (dealButton != null)
            dealButton.interactable = cartManager.CanAffordDeal();
    }

    private void UpdateBadge()
    {
        int count = cartManager.GetTotalItemCount();
        if (cartBadge != null)
            cartBadge.SetActive(count > 0);
        if (cartBadgeText != null)
            cartBadgeText.text = count.ToString();
    }

    private void OnMoneyChanged(int _)
    {
        UpdateDealButton();
    }

    private void OnDealClicked()
    {
        if (!cartManager.PlaceDeal())
            Debug.Log("Не удалось оформить сделку");
    }

    private void OnDestroy()
    {
        if (cartManager != null)
            cartManager.OnCartChanged -= Refresh;
        if (moneyManager != null)
            moneyManager.OnMoneyChanged -= OnMoneyChanged;
        if (dealButton != null)
            dealButton.onClick.RemoveListener(OnDealClicked);
    }
}

/// <summary>
/// Строка товара в панели корзины: имя, количество, +/-.
/// </summary>
public class CartLineItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;

    private ProductData product;
    private CartManager cartManager;

    public void Setup(ProductData data, CartManager cart)
    {
        product = data;
        cartManager = cart;
        BindIfNeeded();

        nameText.text = data.itemName;
        RefreshQuantity();

        minusButton.onClick.RemoveListener(OnMinusClicked);
        plusButton.onClick.RemoveListener(OnPlusClicked);
        minusButton.onClick.AddListener(OnMinusClicked);
        plusButton.onClick.AddListener(OnPlusClicked);
    }

    private void BindIfNeeded()
    {
        if (nameText != null) return;

        nameText = transform.Find("ProductName")?.GetComponent<TextMeshProUGUI>();
        quantityText = transform.Find("BuyZone/ItemsCount")?.GetComponent<TextMeshProUGUI>();

        var quantityRoot = transform.Find("Quantity");
        if (quantityRoot != null)
        {
            minusButton = quantityRoot.Find("Minus")?.GetComponent<Button>();
            plusButton = quantityRoot.Find("Plus")?.GetComponent<Button>();
        }
    }

    private void RefreshQuantity()
    {
        int qty = cartManager.GetQuantity(product);
        quantityText.text = $"x{qty}";
    }

    private void OnMinusClicked()
    {
        cartManager.RemoveFromCart(product);
    }

    private void OnPlusClicked()
    {
        cartManager.AddToCart(product);
    }

    private void OnDestroy()
    {
        if (minusButton != null)
            minusButton.onClick.RemoveListener(OnMinusClicked);
        if (plusButton != null)
            plusButton.onClick.RemoveListener(OnPlusClicked);
    }
}
