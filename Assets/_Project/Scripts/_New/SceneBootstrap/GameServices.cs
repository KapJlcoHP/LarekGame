using UnityEngine;
// ��� ������, ������� ����� ��������� � ������� DontDestroyOnLoad � ����� � ��������� ��� ����������� ���������� ��� ����
public class GameServices : MonoBehaviour
{
    public SaveManager saveManager { get; set; }
    public OrderManager orderManager { get; set; }
    public PlayerManager playerManager { get; set; }
    public MoneyManager moneyManager { get; set; }
    public ProductManager productManager { get; set; }
    public ShopManager shopManager{ get; set; }
    public CartManager cartManager { get; set; }
    public DealSpawner dealSpawner { get; set; }
}
