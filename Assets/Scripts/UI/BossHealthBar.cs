using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : StatesBar_HUD
{
    protected override void SetPercentText()
    {
        // Boss 血条显示小数点后两位
        // percentText.text = string.Format("{0:N2}", targetFillAmount * 100f) + "%";
        // percentText.text = (targetFillAmount * 100f).ToString("f2") + "%";
        percentText.text = targetFillAmount.ToString("P2");  // 百分制两位小数
    }
}
