using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatesBar_HUD : StatesBar
{
    [SerializeField] protected Text percentText;

    protected virtual void SetPercentText() {
        // 重载为字符串之间的相加
        // percentText.text = Mathf.RoundToInt(targetFillAmount * 100) + "%";
        // P -> Percent 0 : 0 位小数
        percentText.text = targetFillAmount.ToString("P0");  // 同样能起到转化为没有小数的百分比值的功能
    }

    public override void Initialize(float currentValue, float maxValue)
    {
        base.Initialize(currentValue, maxValue);
        SetPercentText();
    }

    public override void UpdateStates(float currentValue, float maxValue)
    {
        base.UpdateStates(currentValue, maxValue);
        SetPercentText();
    }

}
