using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class SceneBootstrap : MonoBehaviour
{
    private GameServices gameServices;
    async void Start()
    {
        GameObject mainService = new GameObject("GameServices");
        //Instantiate(mainService);
        DontDestroyOnLoad(mainService);
        mainService.AddComponent<GameServices>();
        gameServices = mainService.GetComponent<GameServices>();
        gameServices.productManager = new ProductManager();
        gameServices.moneyManager = new MoneyManager();
        gameServices.playerManager = new PlayerManager();
        await InitProductManager("Assets/_Project/ItemsSO/FirstLevel.asset");
        await InitMoneyManager();
        await InitScene("Assets/_Project/Prefabs/SceneSO/Level0.asset");
        // await LoadMainMenu();
        Destroy(gameObject);

    }

    async Task InitProductManager(string key)
    {
        var dbHandle = Addressables.LoadAssetAsync<ProductDatabase>(key);
        await dbHandle.Task;
        if (dbHandle.Status == AsyncOperationStatus.Succeeded)
        {
            await gameServices.productManager.AddCatalog(dbHandle.Result);
            Addressables.Release(dbHandle);
        }
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
    async Task InitScene(string key)
    {
        var levelHandle = Addressables.LoadAssetAsync<ScenePrefab>(key);
        await levelHandle.Task;
        if(levelHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var sceneData = levelHandle.Result;
            var envHandle = sceneData.scenePref.LoadAssetAsync();
            await envHandle.Task;
            var env = Instantiate(envHandle.Result);
            Addressables.Release(envHandle);
            var playerHandle = sceneData.playerPref.LoadAssetAsync();
            await playerHandle.Task;
            var player = Instantiate(playerHandle.Result, env.transform);
            Addressables.Release(playerHandle);
            player.transform.localPosition = sceneData.playerPos;
            //gameServices.playerManager.SetPlayer(player);
        }
        else
        {
            Debug.LogError("Не удалось загрузить сцену");
        }
        Addressables.Release(levelHandle);
    }
    Task InitMoneyManager()
    {
        gameServices.moneyManager.SetMoney(1000);
        return Task.CompletedTask;
    }
}