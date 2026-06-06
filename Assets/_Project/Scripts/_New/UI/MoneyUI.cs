using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MoneyUI : UIWindow
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private MoneyManager moneyManager;

    public void Init(MoneyManager mm, WindowManager wm)
    {
        base.Init(wm);                  // обязательно для UIWindow
        moneyManager = mm;
        moneyManager.OnMoneyChanged += UpdateMoney;
        UpdateMoney(moneyManager.Balance());
    }

    void UpdateMoney(int newMoney)
    {
        moneyText.text = MoneyFormatter.Format(newMoney);
    }

    void OnDestroy()
    {
        if (moneyManager != null)
            moneyManager.OnMoneyChanged -= UpdateMoney;
    }
}
