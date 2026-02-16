using UnityEngine;
using TMPro;


public class WalletSystem : MonoBehaviour
{
    public int wallet;
    public TMP_Text walletUI;

    private void Start()
    {
        walletUI.text = "Money: " + wallet.ToString();
    }



    public void AddMoney(int money)
    {
        wallet += money;
        walletUI.text = "Money: " + wallet.ToString();
    }
    public void RemoveMoney(int money)
    {
        wallet -= money;
        walletUI.text = "Money: " + wallet.ToString();
    }

}
