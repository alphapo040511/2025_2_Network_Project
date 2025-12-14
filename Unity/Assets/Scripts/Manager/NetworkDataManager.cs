using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

[System.Serializable]
public class NetworkItemData
{
    public string itemKey;
    public int quantity;
}

public class NetworkDataManager : SingletonMonoBehaviour<NetworkDataManager>
{
    private Player playerData;

    public void SetPlayerData(int player_id, string username)
    {
        Player playerData = new Player();
        playerData.player_id = player_id;
        playerData.username = username;
    }

    public List<NetworkItemData> LoadInventoryData()
    {
        List<NetworkItemData> inventory = new List<NetworkItemData>();


        return inventory;
    }
}
