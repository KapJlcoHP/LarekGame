using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;

public class WalletSystem : MonoBehaviour
{
    public int wallet;
    public TMP_Text walletUI;
    public List<string> suffixes = new List<string>{ "","K","M","B","T","Qa","Qi","Sx", "Sp", "Oc" };
    
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
