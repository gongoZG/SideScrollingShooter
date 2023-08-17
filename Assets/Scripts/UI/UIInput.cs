using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

/// <summary>
/// 挂载在 EventSystem 上，用于控制 UI 的输入
/// </summary>
public class UIInput : Singleton<UIInput>
{
    [SerializeField] PlayerInput playerInput;
    InputSystemUIInputModule UIInputModule;

    protected override void Awake()
    {
        base.Awake();
        UIInputModule = GetComponent<InputSystemUIInputModule>();  // 用于处理 UI 与玩家输入的组件
        UIInputModule.enabled = false;  // 用到的时候再启用
    }

    /// <summary>
    /// 选中一个 UI 并启用 UI 输入
    /// </summary>
    /// <param name="UIObject"></param>
    public void SelectUI(Selectable UIObject) {
        // Selectable：所有可被选中的 UI 对象的基类
        UIObject.Select();
        UIObject.OnSelect(null);  // 设置 UI 为选中状态
        UIInputModule.enabled = true;  // 启用 UI 输入
    }

    /// <summary>
    /// 禁用所有 UI 输入
    /// </summary>
    public void DisableAllUIInputs() {
        playerInput.DisableAllInputs();  // 禁用所有输入
        UIInputModule.enabled = false;  // 禁用 UI 输入
    }
}
