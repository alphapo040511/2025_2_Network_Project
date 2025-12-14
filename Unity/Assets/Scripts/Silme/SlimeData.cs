using UnityEngine;

[CreateAssetMenu(fileName = "NewSlime", menuName = "Game/SlimeData")]
public class SlimeData : ScriptableObject
{
    [Header("기본 정보")]
    public string slimeName;       // 슬라임 이름
    public int price;              // 구매 가격
    public GameObject prefab;      // 소환될 슬라임 프리팹

    [Header("경제 활동")]
    public float incomeTime;       // 돈을 버는 시간 간격 (초)
    public int incomeAmount;       // 한 번에 버는 돈의 양
}