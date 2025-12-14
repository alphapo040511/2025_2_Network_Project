using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlimeListView : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;
    public Image iconImage;

    public void Show(string slimeName, float amout, Sprite icon)
    {
        nameText.text = slimeName;
        amountText.text = $"{amout:F1}G/sec";
        iconImage.sprite = icon;
    }
}
