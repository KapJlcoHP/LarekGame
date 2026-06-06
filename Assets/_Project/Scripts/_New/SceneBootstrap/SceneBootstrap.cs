using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneBootstrap : MonoBehaviour
{
    private GameServices gameServices;
    private SaveManager saveManager;
    [SerializeField] private UIRoot uiRoot;
    async void Start()
    {
        // 1. ������ GameServices
        await InitGameServices();

        // 2. ������ ������ ��������� (��� ��������� ��������)
        CreateManagers();

        // 3. ������ SaveManager � ������������� �� �������
        saveManager = new SaveManager();
        saveManager.Initialize(
            gameServices.moneyManager,
            gameServices.productManager,
            gameServices.orderManager,
            gameServices.moneyManager,
            gameServices.productManager,
            gameServices.orderManager
        );

        // 4. ��������� ���������� (��� �������� �� ���������)
        GameSaveData loadedData = saveManager.Load();

        // 5. ��������� ������ ����� (�� ������� �� ��������)
        gameServices.moneyManager.LoadSaveData(loadedData.moneyData);

        // 6. ��������� ������� ���������
        await LoadProductCatalog("Assets/_Project/ItemsSO/FirstLevel.asset");

        // 7. ��������� ������������� (������ ������� ��������)
        gameServices.productManager.LoadSaveData(loadedData.productData);

        // 8. ��������������� ��� ���������� �����
        if (loadedData.orderData.productIds.Length > 0)
            gameServices.orderManager.LoadSaveData(loadedData.orderData);
        else
            gameServices.orderManager.GenerateOrder();  // ������� ������� -> ����������

        // 9. ��������� ����� (��������� + �����)
        await InitScene("Assets/_Project/Prefabs/SceneSO/Level0.asset");
        uiRoot.Initialize(gameServices);
        // 10. ���� Bootstrap ������ �� �����
        Destroy(gameObject);
    }

    // �������� ���������� GameServices
    async Task InitGameServices()
    {
        GameObject mainService = new GameObject("GameServices");
        DontDestroyOnLoad(mainService);
        mainService.AddComponent<GameServices>();
        gameServices = mainService.GetComponent<GameServices>();
        await Task.CompletedTask;
    }

    // �������� ����������� ���������� (���� ��� ������)
    void CreateManagers()
    {
        void CreateManagers()
        {
            gameServices.moneyManager = new MoneyManager();
            gameServices.productManager = new ProductManager();
            gameServices.orderManager = new OrderManager();
            gameServices.orderManager.SetMoneyManager(gameServices.moneyManager);
            gameServices.orderManager.SetProductManager(gameServices.productManager);
            gameServices.cartManager = new CartManager(gameServices.moneyManager);
            gameServices.shopManager = new ShopManager(
                gameServices.productManager, 
                gameServices.moneyManager
            );
        }
    }

    // �������� �������� ��������� ����� Addressables
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
            Debug.LogError("�� ������� ��������� �� ���������");
        }
    }

    // �������� ����� ������ (��������� + �����)
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

            // �����
            await InitPlayerManager();
            await gameServices.playerManager.SpawnPlayer(sceneData);
        }
        else
        {
            Debug.LogError("�� ������� ��������� �����");
        }
        Addressables.Release(levelHandle);
    }

    // �������� PlayerManager (�� MonoBehaviour, ������ ���� �� GameServices)
    async Task InitPlayerManager()
    {
        gameServices.gameObject.AddComponent<PlayerManager>();
        gameServices.playerManager = gameServices.gameObject.GetComponent<PlayerManager>();
        await Task.CompletedTask;
    }
}