using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameMana : MonoBehaviour
{
    // [변경 사항] 싱글톤 인스턴스 이름 수정
    public static GameMana Instance;

    [Header("플레이어 상태")]
    public int currentMoney = 20; // 테스트를 위해 넉넉히 20원 시작
    public int rerollCost = 2;    // 리롤 비용

    [Header("데이터베이스")]
    public List<SlimeDataSO> allSlimeData;

    [Header("UI 연결")]
    public List<ShopSlot> shopSlots;
    public TextMeshProUGUI moneyText;

    [Header("생성 위치")]
    public Transform spawnPoint;

    [Header("필수 할당")]
    public Transform viewportContent; // Scroll View 안의 'Content' 오브젝트 연결
    public SlimeListView itemPrefab;     // 생성할 슬롯(버튼/이미지 등) 프리팹

    public NetworkManager chatManager;

    private void Awake()
    {
        // [변경 사항] 싱글톤 초기화 시 클래스 이름 수정
        Instance = this;
    }

    private void Start()
    {
        SlimeGenerator();
        InitializeMoney();
        RerollShop();
    }

    void InitializeMoney()
    {
        StartCoroutine(NetworkDataManager.Instance.FetchGold((gold) =>
        {
            currentMoney = gold;
            UpdateUI();
        }));
    }

    // --- 새로고침 버튼 기능 ---
    public void OnClickReroll()
    {
        Debug.Log("새로고침 버튼 눌림!");

        if (currentMoney >= rerollCost)
        {
            currentMoney -= rerollCost;
            StartCoroutine(NetworkDataManager.Instance.GoldUpdate(currentMoney));   // 골드 서버에 반영
            UpdateUI();
            RerollShop();
        }
        else
        {
            Debug.Log("돈이 부족해서 새로고침 실패!");
        }
    }

    private void RerollShop()
    {
        StartCoroutine(NetworkDataManager.Instance.SlimeGachaDraw(shopSlots.Count, (slimes) =>
        {
            if(slimes != null)
                SetSlots(slimes);
        }));
    }

    public void SetSlots(List<NetworkItemData> slimeDatas)
    {
        int length = Mathf.Max(slimeDatas.Count, shopSlots.Count);
        for(int i = 0; i < length; i++)
        {
            SlimeDataSO silme = InventoryManager.Instance.GetSlimeToKey(slimeDatas[i].slime_key);

            if(silme != null)
                shopSlots[i].SetSlot(silme);
        }
    }
    // ------------------------------------

    public bool TryBuySlime(SlimeDataSO data)
    {
        if (currentMoney >= data.price)
        {
            currentMoney -= data.price;     // 골드 사용

            StartCoroutine(NetworkDataManager.Instance.GoldUpdate(currentMoney));   // 골드 서버에 반영

            InventoryManager.Instance.AddSlime(data);       // 슬라임 추가

            if(chatManager != null && NetworkDataManager.Instance.playerData != null)
            {
                chatManager.SendChatMessage($"[시스템] <color=#4DD0E1>{NetworkDataManager.Instance.playerData.username}</color> 님이 {data.slimeName}을 획득 하였습니다.");
            }

            UpdateUI();

            Vector3 randomPos = spawnPoint.position + new Vector3(Random.Range(-30f, 20f), 1, Random.Range(-30f, 20f));
            GameObject go = Instantiate(data.prefab, randomPos, Quaternion.identity);

            Slime slimeScript = go.GetComponent<Slime>();
            if (slimeScript != null) slimeScript.Initialize(data);

            AddSlimeList(data);

            return true;
        }
        return false;
    }

    void AddSlimeList(SlimeDataSO data)
    {
        SlimeListView view = Instantiate(itemPrefab, viewportContent);

        float goldPerSec = data.incomeTime == 0 ? 0 : data.incomeAmount / data.incomeTime;

        view.Show(data.slimeName, goldPerSec, data.slimeImage);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"Gold: {currentMoney}";
    }

    public void SlimeGenerator()
    {
        List<ItemData> itemInven = InventoryManager.Instance.GetInventoryData();
        foreach(ItemData item in itemInven)
        {
            AddSlimeList(item.slimeData);

            for (int i = 0; i < item.quantity; i++)
            {
                SpawnSlime(item.slimeData);

                Debug.Log($"{item.slimeData.slimeName}/{item.quantity}");
            }
        }
    }

    void SpawnSlime(SlimeDataSO data)
    {
        if (data.prefab == null) return;

        // 위치를 약간 랜덤하게 흩뿌리기
        Vector3 randomPos = spawnPoint.position + new Vector3(Random.Range(-30f, 20f), 1, Random.Range(-30f, 20f));

        // 생성!
        GameObject go = Instantiate(data.prefab, randomPos, Quaternion.identity);

    }
}