using UnityEngine;
using UnityEngine.UI;

public enum SlimeRarity
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendery = 3,
    Mystic = 4
}

[CreateAssetMenu(fileName = "NewSlime", menuName = "Game/SlimeData")]
public class SlimeDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string key;             // 슬라임 구분용 Key
    public string slimeName;       // 슬라임 이름
    public int price;              // 구매 가격
    public SlimeRarity slimeRarity;// 슬라임 희귀도
    public GameObject prefab;      // 소환될 슬라임 프리팹
    public Image slimeImage;      // 소환될 슬라임 이미지

    [Header("경제 활동")]
    public float incomeTime;       // 돈을 버는 시간 간격 (초)
    public int incomeAmount;       // 한 번에 버는 돈의 양
}

public class ItemData
{
    public SlimeDataSO slimeData;
    public int quantity;

    public ItemData(SlimeDataSO data, int quantity = 0)
    {
        this.slimeData = data;
        this.quantity = quantity;
    }
}