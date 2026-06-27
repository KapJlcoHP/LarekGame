using NUnit;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
     public async Task SpawnPlayer(ScenePrefab sceneData)
    {
        var playerHandle = sceneData.playerPref.LoadAssetAsync();
        await playerHandle.Task;
        var player = Instantiate(playerHandle.Result, Vector3.zero, new Quaternion(0,0,0,0));
        Addressables.Release(playerHandle);
        player.transform.localPosition = sceneData.playerPos;
        await Task.CompletedTask;
    }
}
