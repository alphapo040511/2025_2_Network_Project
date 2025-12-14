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

    private void Awake()
    {
        // [변경 사항] 싱글톤 초기화 시 클래스 이름 수정
        Instance = this;
    }

    private void Start()
    {
        SlimeGenerator();
        UpdateUI();
        RerollShop();
    }

    // --- 새로고침 버튼 기능 ---
    public void OnClickReroll()
    {
        Debug.Log("새로고침 버튼 눌림!");

        if (currentMoney >= rerollCost)
        {
            currentMoney -= rerollCost;
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
        // 상점 슬롯을 모두 새로운 랜덤 슬라임으로 덮어쓰기
        foreach (var slot in shopSlots)
        {
            if (allSlimeData.Count == 0) continue;

            int randomIndex = Random.Range(0, allSlimeData.Count);
            SlimeDataSO randomSlime = allSlimeData[randomIndex];

            slot.SetSlot(randomSlime);
        }
    }
    // ------------------------------------

    public bool TryBuySlime(SlimeDataSO data)
    {
        if (currentMoney >= data.price)
        {
            currentMoney -= data.price;
            UpdateUI();

            Vector3 randomPos = spawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            GameObject go = Instantiate(data.prefab, randomPos, Quaternion.identity);

            Slime slimeScript = go.GetComponent<Slime>();
            if (slimeScript != null) slimeScript.Initialize(data);



            return true;
        }
        return false;
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
            for (int i = 0; i < item.quantity; i++)
            {
                SpawnSlime(item.slimeData);
            }
        }
    }

    void SpawnSlime(SlimeDataSO data)
    {
        if (data.prefab == null) return;

        // 위치를 약간 랜덤하게 흩뿌리기
        Vector3 randomPos = spawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));

        // 생성!
        GameObject go = Instantiate(data.prefab, randomPos, Quaternion.identity);

    }
}