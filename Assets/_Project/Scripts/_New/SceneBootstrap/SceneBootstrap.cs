using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class SceneBootstrap : MonoBehaviour
{
    private ProductManager prodManager;
    private MoneyManager moneyManager;
    private AsyncOperationHandle<ProductDatabase> dbHandle;

    async void Start()
    {
        await InitProductManager("Assets/_Project/ItemsSO/FirstLevel.asset");
        await InitMoneyManager();
       // await LoadMainMenu();

    }

    async Task InitProductManager(string key)
    {
        if (FindFirstObjectByType<ProductManager>() == null)
        {
            var go = new GameObject("ProductManager");
            DontDestroyOnLoad(go);
            prodManager = go.AddComponent<ProductManager>();
        }
        else
        {
            prodManager = FindFirstObjectByType<ProductManager>();
        }

        dbHandle = Addressables.LoadAssetAsync<ProductDatabase>(key);
        await dbHandle.Task;

        if (dbHandle.Status == AsyncOperationStatus.Succeeded)
            prodManager.AddCatalog(dbHandle.Result);
        else
            Debug.LogError("Не удалось загрузить БД");
    }

    /*async Task LoadMainMenu()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("ui/main_menu");
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var menuObj = Instantiate(handle.Result);
            //menuObj.GetComponent<MainMenuUI>().Init(prodManager);
        }
    }*/
    async Task InitMoneyManager()
    {
        
        if (FindFirstObjectByType<MoneyManager>() == null)
        {
            var go = new GameObject("MoneyManager");
            DontDestroyOnLoad(go);
            moneyManager = go.AddComponent<MoneyManager>();
        }
        else
        {
            moneyManager = FindFirstObjectByType<MoneyManager>();
        }
        moneyManager.SetMoney(1000);
    }
    void OnDestroy()
    {
        if (dbHandle.IsValid())
            Addressables.Release(dbHandle);
    }
}