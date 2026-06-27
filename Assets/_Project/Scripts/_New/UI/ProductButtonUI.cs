using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Карточка одного продукта в списке магазина.
/// Два состояния: Заблокирован (показывает оверлей с ценой разблокировки, кнопка "Разблокировать")
/// и Разблокирован (показывает цену покупки, кнопка "Добавить в корзину", индикатор количества в корзине).
/// </summary>
public class ProductButtonUI : MonoBehaviour
{
    [Header("Основные элементы")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;        // основная цена (меняется в зависимости от состояния)
    [SerializeField] private Button actionButton;
    [SerializeField] private Button unlockButton;
    [Header("Оверлей блокировки")]
    [SerializeField] private GameObject lockedOverlay;          // красный полупрозрачный фон с замком
    [SerializeField] private TextMeshProUGUI unlockPriceText;  // цена разблокировки на оверлее

    [Header("Индикатор корзины")]
    [SerializeField] private GameObject cartIndicator;          // кружок/панелька с цифрой
    [SerializeField] private TextMeshProUGUI cartCountText;    // текст количества

    // Данные продукта и зависимости
    private ProductData product;
    private ProductManager productManager;
    private MoneyManager moneyManager;
    private CartManager cartManager;
    private ShopManager shopManager;

    private AsyncOperationHandle<Sprite> iconHandle;

    /// <summary>
    /// Вызывается из ProductListUI после Instantiate.
    /// Настраивает карточку, подписывается на события.
    /// </summary>
    public void Setup(ProductData data, ProductManager pm, MoneyManager mm, CartManager cm, ShopManager sm)
    {
        product = data;
        productManager = pm;
        moneyManager = mm;
        cartManager = cm;
        shopManager = sm;

        // Заполняем неизменяемые данные
        nameText.text = data.itemName;

        // Загружаем иконку (Addressables)
        if (data.icon != null && data.icon.RuntimeKeyIsValid())
        {
            iconHandle = data.icon.LoadAssetAsync();
            iconHandle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    iconImage.sprite = handle.Result;
            };
        }

        // Подписываемся на внешние события
        moneyManager.OnMoneyChanged += OnMoneyChanged;
        productManager.OnProductUnlocked += OnProductUnlocked;
        cartManager.OnCartChanged += OnCartChanged;

        actionButton.onClick.RemoveListener(OnActionButtonClicked);
        unlockButton.onClick.RemoveListener(OnActionButtonClicked);
        actionButton.onClick.AddListener(OnActionButtonClicked);
        unlockButton.onClick.AddListener(OnActionButtonClicked);

        RefreshState();
    }

    /// <summary>
    /// Переключает визуал в зависимости от того, разблокирован ли продукт.
    /// </summary>
    private void RefreshState()
    {
        bool isUnlocked = productManager.IsUnlocked(product);

        if (isUnlocked)
            SetUnlockedState();
        else
            SetLockedState();

        // Обновляем индикатор корзины (если продукт разблокирован, он может быть в корзине)
        UpdateCartIndicator();
        // Обновляем доступность кнопок с учётом баланса
        UpdateInteractable(moneyManager.Balance());
    }

    /// <summary>
    /// Визуал для заблокированного продукта.
    /// </summary>
    private void SetLockedState()
    {
        lockedOverlay.SetActive(true);
        unlockPriceText.text = $"Unlock\n{MoneyFormatter.Format(product.unlockPrice)}";
        cartIndicator.SetActive(false); // в корзину нельзя добавить заблокированный продукт
    }

    /// <summary>
    /// Визуал для разблокированного продукта.
    /// </summary>
    private void SetUnlockedState()
    {
        lockedOverlay.SetActive(false);
        priceText.text = MoneyFormatter.Format(product.buyPrice); // теперь цена покупки

        // индикатор корзины может показываться, если продукт уже добавлен
    }

    /// <summary>
    /// Обновляет активность кнопки действия в зависимости от баланса.
    /// Для разблокировки: активна, если денег >= unlockPrice.
    /// Для добавления в корзину: всегда активна (проверка баланса будет при Deal).
    /// </summary>
    private void UpdateInteractable(int currentMoney)
    {
        if (!productManager.IsUnlocked(product))
        {
            bool canAfford = shopManager.CanAffordUnlock(product);
            actionButton.interactable = canAfford;
            unlockButton.interactable = canAfford;
        }
        else
        {
            actionButton.interactable = true;
            unlockButton.interactable = false;
        }
    }

    /// <summary>
    /// Обработчик нажатия на основную кнопку (Разблокировать / Добавить в корзину).
    /// </summary>
    public void OnActionButtonClicked()
    {
        if (!productManager.IsUnlocked(product))
        {
            // Используем ShopManager для разблокировки
            bool success = shopManager.TryUnlockProduct(product);
            if (!success)
            {
                Debug.Log("Не удалось разблокировать товар");
                // Можно показать всплывашку "Недостаточно денег"
            }
            // Если success == true, ProductManager кинет событие OnProductUnlocked,
            // и RefreshState() вызовется автоматически через подписку
        }
        else
        {
            // Добавить в корзину
            cartManager.AddToCart(product);
        }
    }

    /// <summary>
    /// Вызывается при изменении баланса.
    /// </summary>
    private void OnMoneyChanged(int newMoney)
    {
        UpdateInteractable(newMoney);
    }

    /// <summary>
    /// Вызывается, когда какой-либо продукт разблокирован.
    /// </summary>
    private void OnProductUnlocked(ProductData unlocked)
    {
        if (unlocked == product)
        {
            // Продукт только что разблокирован — переключаем визуал
            RefreshState();
        }
    }

    /// <summary>
    /// Вызывается при любом изменении корзины.
    /// </summary>
    private void OnCartChanged()
    {
        UpdateCartIndicator();
    }

    /// <summary>
    /// Обновляет индикатор количества в корзине (показывается только для разблокированных продуктов).
    /// </summary>
    private void UpdateCartIndicator()
    {
        if (!productManager.IsUnlocked(product))
        {
            cartIndicator.SetActive(false);
            return;
        }

        int qty = cartManager.GetQuantity(product);
        if (qty > 0)
        {
            cartCountText.text = qty.ToString();
            cartIndicator.SetActive(true);
        }
        else
        {
            cartIndicator.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (actionButton != null)
            actionButton.onClick.RemoveListener(OnActionButtonClicked);
        if (unlockButton != null)
            unlockButton.onClick.RemoveListener(OnActionButtonClicked);

        if (moneyManager != null)
            moneyManager.OnMoneyChanged -= OnMoneyChanged;
        if (productManager != null)
            productManager.OnProductUnlocked -= OnProductUnlocked;
        if (cartManager != null)
            cartManager.OnCartChanged -= OnCartChanged;

        // Освобождаем хендл иконки
        if (iconHandle.IsValid())
            Addressables.Release(iconHandle);
    }
}