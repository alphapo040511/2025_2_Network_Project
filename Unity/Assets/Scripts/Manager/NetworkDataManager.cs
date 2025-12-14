using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using System;


[System.Serializable]
public class NetworkItemData
{
    public string slime_key;
    public int quantity;
}

public class NetworkDataManager : SingletonMonoBehaviour<NetworkDataManager>
{
    private const string INVENTORY_SERVER_URL = "http://localhost:4001";
    private const string GACHA_SERVER_URL = "http://localhost:4002";

    public Player playerData { get; private set; }

    public void SetPlayerData(int player_id, string username)
    {
        Player playerData = new Player();
        playerData.player_id = player_id;
        playerData.username = username;

        this.playerData = playerData;
    }

    // 서버의 인벤토리 데이터 로드
    public IEnumerator FetchInventory()
    {
        if(playerData == null) yield break;

        using (UnityWebRequest www = UnityWebRequest.Get($"{INVENTORY_SERVER_URL}/inventory/{playerData.player_id}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<NetworkItemData> playerSlimes = new List<NetworkItemData>();

                playerSlimes = JsonConvert.DeserializeObject<List<NetworkItemData>>(www.downloadHandler.text);
                foreach (var slime in playerSlimes)
                {
                    Debug.Log($"\n - {slime.slime_key} 슬라임 | {slime.quantity} 개");
                }

                InventoryManager.Instance.SetInventoryItems(playerSlimes);

                Debug.Log($"인벤토리 정리 완료 {playerSlimes.Count} 개");
            }
            else
            {
                Debug.LogWarning("인벤토리 슬라임 데이터 패치 오류");
            }
        }
    }


    // 서버의 인벤토리 데이터 로드
    public IEnumerator SlimeGachaDraw(int count, Action<List<NetworkItemData>> onComplete)
    {
        Debug.Log("랜덤 슬라임 반환 시도!");
        if (playerData == null) yield break;

        using (UnityWebRequest www = UnityWebRequest.Get($"{GACHA_SERVER_URL}/gacha/refresh/{count}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var shopItems = JsonConvert.DeserializeObject<List<NetworkItemData>>(www.downloadHandler.text);
                Debug.Log($"랜덤 슬라임 {shopItems.Count} 개를 반환");
                onComplete?.Invoke(shopItems);
            }
            else
            {
                Debug.LogWarning("상점 리프래쉬 실패");
                onComplete?.Invoke(null);
            }
        }
    }
}
