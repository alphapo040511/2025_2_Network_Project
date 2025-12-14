using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using System;
using System.Text;


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

    // 슬라임 획득 DB에 업데이트
    public IEnumerator AddSlime(SlimeDataSO slime)
    {
        Debug.Log($"슬라임 추가 시도 : {slime.slimeName}");
        if (playerData == null) yield break;

        var data = new { player_id = playerData.player_id, slime_key = slime.key };
        var jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest www = new UnityWebRequest($"{INVENTORY_SERVER_URL}/inventory/getslime", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("[슬라임] 저장 성공");
            else
            {
                BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(www.downloadHandler.text);
                Debug.LogWarning($"[슬라임] 저장 실패 {baseResponse.message}");
            }
        }
    }


    // 매 초 획득 골드 동기화는 너무 무거울 것 같아서..
    // 일단 사용 할 때 라도 업데이트 (클라이언트 변조를 한다면 문제가 있겠지만 지금은 이렇게 해둘게요..)
    public IEnumerator GoldUpdate(int amount)
    {
        Debug.Log($"골드 소지량 업데이트 시도 : {amount}G");
        if (playerData == null) yield break;

        var data = new { player_id = playerData.player_id, gold = amount };
        var jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest www = new UnityWebRequest($"{INVENTORY_SERVER_URL}/gold/update", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("[골드] 저장 성공");
            else
                Debug.LogWarning("[골드] 저장 실패");
        }
    }

    public IEnumerator FetchGold(Action<int> callback)
    {
        Debug.Log("골드 패치 시도");
        if (playerData == null) yield break;

        using (UnityWebRequest www = UnityWebRequest.Get($"{INVENTORY_SERVER_URL}/gold/{playerData.player_id}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<int>(www.downloadHandler.text);
                Debug.Log($"보유 골드량 : {response}G");
                callback?.Invoke(response);
            }
            else
            {
                Debug.LogWarning("골드 패치 실패");
                callback?.Invoke(50);
            }
        }
    }
}
