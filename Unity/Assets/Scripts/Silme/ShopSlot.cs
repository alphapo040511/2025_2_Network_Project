using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Image slimeImage;
    public Button buyButton;       // 구매 버튼
    public GameObject soldPanel;   // (선택) "판매됨"이라고 가려주는 패널이 있다면 사용

    private SlimeDataSO currentData;

    // GameManager가 이 함수를 호출해서 슬롯을 채워줍니다.
    public void SetSlot(SlimeDataSO data)
    {
        currentData = data;

        if (data != null)
        {
            // [중요] 상태 완전 초기화 (새로고침 시 필수!)
            nameText.text = data.slimeName;
            priceText.text = $"{data.price}G";
            slimeImage.sprite = data.slimeImage;

            buyButton.interactable = true; 
            // soldPanel이 있다면 비활성화: if(soldPanel) soldPanel.SetActive(false);
            gameObject.SetActive(true);
        }
        else
        {
            // 데이터가 없으면 슬롯 숨기기
            gameObject.SetActive(false);
        }
    }

    public void OnClickBuy()
    {
        if (currentData == null) return;

        // 매니저에게 구매 요청
        bool success = GameMana.Instance.TryBuySlime(currentData);

        if (success)
        {
            // 구매 성공 처리 (롤체처럼 비활성화)
            buyButton.interactable = false; // 버튼 끄기
            nameText.text = "SOLDOUT";      // 텍스트 변경
            priceText.text = "-";

            currentData = null; // 데이터 비우기 (중복 구매 방지)
        }
    }
}