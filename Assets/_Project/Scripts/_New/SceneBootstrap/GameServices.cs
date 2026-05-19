using UnityEngine;
// Это скрипт, который будет находится в статусе DontDestroyOnLoad в сцене и содержать всю необходимую информацию для игры
public class GameServices : MonoBehaviour
{
    public PlayerManager playerManager { get; set; }
    public MoneyManager moneyManager { get; set; }
    public ProductManager productManager { get; set; }
}
