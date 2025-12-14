using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>
{
    public List<SlimeDataSO> slimeSO = new List<SlimeDataSO>();
    private Dictionary<string, SlimeDataSO> slimes = new Dictionary<string, SlimeDataSO>();     // 인벤토리에 슬라임 추가가 용이하도록
    private Dictionary<string, ItemData> inventory = new Dictionary<string, ItemData>();

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        foreach(SlimeDataSO s in slimeSO)
        {
            slimes.Add(s.key, (s));
        }
    }

    public void FetchInvneory()
    {
        StartCoroutine(NetworkDataManager.Instance.FetchInventory());
    }

    public void SetInventoryItems(List<NetworkItemData> items)
    {
        inventory.Clear();
        foreach(NetworkItemData item in items)
        {
            if (!inventory.ContainsKey(item.slime_key) && slimes.ContainsKey(item.slime_key))
            {
                ItemData slime = new ItemData(slimes[item.slime_key], item.quantity);

                inventory.Add(item.slime_key, slime);        // 인벤토리에 추가
            }
        }
    }

    public List<ItemData> GetInventoryData()
    {
        return new List<ItemData>(inventory.Values);        // 인벤토리에 보유중인 (슬라임, 개수) 반환
    }
}
