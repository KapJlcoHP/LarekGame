using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneBootstrap : MonoBehaviour
{
    private GameServices gameServices;
    private SaveManager saveManager;

    async void Start()
    {
        // 1. Создаём GameServices
        await InitGameServices();

        // 2. Создаём пустые менеджеры (без начальных значений)
        CreateManagers();

        // 3. Создаём SaveManager и подписываемся на события
        saveManager = new SaveManager();
        saveManager.Initialize(
            gameServices.moneyManager,
            gameServices.productManager,
            gameServices.orderManager,
            gameServices.moneyManager,
            gameServices.productManager,
            gameServices.orderManager
        );

        // 4. Загружаем сохранение (или значения по умолчанию)
        GameSaveData loadedData = saveManager.Load();

        // 5. Применяем деньги сразу (не зависит от каталога)
        gameServices.moneyManager.LoadSaveData(loadedData.moneyData);

        // 6. Загружаем каталог продуктов
        await LoadProductCatalog("Assets/_Project/ItemsSO/FirstLevel.asset");

        // 7. Применяем разблокировки (теперь каталог загружен)
        gameServices.productManager.LoadSaveData(loadedData.productData);

        // 8. Восстанавливаем или генерируем заказ
        if (loadedData.orderData.productIds.Length > 0)
            gameServices.orderManager.LoadSaveData(loadedData.orderData);
        else
            gameServices.orderManager.GenerateOrder();  // вызовет событие -> сохранится

        // 9. Загружаем сцену (окружение + игрок)
        await InitScene("Assets/_Project/Prefabs/SceneSO/Level0.asset");

        // 10. Этот Bootstrap больше не нужен
        Destroy(gameObject);
    }

    // Создание контейнера GameServices
    async Task InitGameServices()
    {
        GameObject mainService = new GameObject("GameServices");
        DontDestroyOnLoad(mainService);
        mainService.AddComponent<GameServices>();
        gameServices = mainService.GetComponent<GameServices>();
        await Task.CompletedTask;
    }

    // Создание экземпляров менеджеров (пока без данных)
    void CreateManagers()
    {
        gameServices.moneyManager = new MoneyManager();
        gameServices.productManager = new ProductManager();
        gameServices.orderManager = new OrderManager();
        gameServices.orderManager.SetMoneyManager(gameServices.moneyManager);
        gameServices.orderManager.SetProductManager(gameServices.productManager);
    }

    // Загрузка каталога продуктов через Addressables
    async Task LoadProductCatalog(string key)
    {
        var dbHandle = Addressables.LoadAssetAsync<ProductDatabase>(key);
        await dbHandle.Task;
        if (dbHandle.Status == AsyncOperationStatus.Succeeded)
        {
            await gameServices.productManager.AddCatalog(dbHandle.Result);
            Addressables.Release(dbHandle);
        }
        else
        {
            Debug.LogError("Не удалось загрузить БД продуктов");
        }
    }

    // Загрузка сцены уровня (окружение + игрок)
    async Task InitScene(string key)
    {
        var levelHandle = Addressables.LoadAssetAsync<ScenePrefab>(key);
        await levelHandle.Task;
        if (levelHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var sceneData = levelHandle.Result;
            var envHandle = sceneData.scenePref.LoadAssetAsync();
            await envHandle.Task;
            Instantiate(envHandle.Result);
            Addressables.Release(envHandle);

            // Игрок
            await InitPlayerManager();
            await gameServices.playerManager.SpawnPlayer(sceneData);
        }
        else
        {
            Debug.LogError("Не удалось загрузить сцену");
        }
        Addressables.Release(levelHandle);
    }

    // Создание PlayerManager (он MonoBehaviour, должен быть на GameServices)
    async Task InitPlayerManager()
    {
        gameServices.gameObject.AddComponent<PlayerManager>();
        gameServices.playerManager = gameServices.gameObject.GetComponent<PlayerManager>();
        await Task.CompletedTask;
    }
}