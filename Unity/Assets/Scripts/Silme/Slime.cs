using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour
{
    private SlimeData data;
    private float timer;

    // 생성될 때 데이터를 주입받음
    public void Initialize(SlimeData slimeData)
    {
        this.data = slimeData;
        timer = 0f;

        // 코루틴으로 돈 벌기 시작
        StartCoroutine(EarnMoneyRoutine());
    }

    IEnumerator EarnMoneyRoutine()
    {
        while (true)
        {
            // 설정된 시간만큼 대기
            yield return new WaitForSeconds(data.incomeTime);

            // 돈 벌기 (GameManager에 돈 추가 요청)
            GameMana.Instance.AddMoney(data.incomeAmount);

            // (선택사항) 돈 벌 때 시각적 효과나 로그
            Debug.Log($"{data.slimeName}이(가) {data.incomeAmount}원을 벌었습니다!");
        }
    }
}